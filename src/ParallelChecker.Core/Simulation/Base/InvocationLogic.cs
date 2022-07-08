using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class InvocationLogic {
    public static void InvokeConstructor(this Program program, Model.Object instance, IMethodSymbol constructor, object[] arguments) {
      var constructorDeclaration = program.CompilationModel.ResolveSyntaxNode<ConstructorDeclarationSyntax>(constructor);
      ConstructorInitializerSyntax initializer = null;
      if (constructorDeclaration != null) {
        program.InvokeMethod(constructor, arguments, instance, null);
        initializer = constructorDeclaration.Initializer;
      } else if (program.CompilationModel.IsRecord(constructor.ContainingType)) {
        InitializePositionalRecord(instance, constructor, arguments);
      }
      var type = constructor.ContainingType;
      if (initializer == null || initializer.Kind() == SyntaxKind.BaseConstructorInitializer) {
        InitializeFields(program, instance, type, type.IsStruct() && constructor.IsImplicitlyDeclared);
      }
      if (initializer == null) {
        var baseType = type.BaseType;
        if (baseType != null && !baseType.Is(Symbols.RootClass)) {
          var baseConstructor = baseType.GetDefaultConstructor();
          if (baseConstructor != null) {
            program.InvokeConstructor(instance, baseConstructor, new object[] { });
          }
        }
      }
    }

    private static void InitializePositionalRecord(Model.Object instance, IMethodSymbol constructor, object[] arguments) {
      var type = constructor.ContainingType;
      if (constructor.Parameters.Length != arguments.Length) {
        throw new NotImplementedException();
      }
      for (int index = 0; index < arguments.Length; index++) {
        var parameter = constructor.Parameters[index];
        if (parameter.RefKind != RefKind.None) {
          throw new NotImplementedException();
        }
        var argument = arguments[index];
        var property = type.GetProperty(parameter.Name);
        if (property != null) {
          instance.InstanceFields[property].Value = argument;
        }
      }
    }

    private static void InitializeFields(Program program, Model.Object instance, ITypeSymbol type, bool structDefault) {
      if (program.CompilationModel.ResolveSyntaxNode<BaseTypeDeclarationSyntax>(type) is TypeDeclarationSyntax typeDeclaration) {
        var routine = new InitializerRoutine(typeDeclaration, structDefault);
        var graph = program.ControlFlowModel.GetGraph(routine);
        var cause = new Cause($"initialization {type.Name}", program.ActiveLocation, program.ActiveCause);
        var callee = new Method(null, graph.Entry, null, cause) {
          ThisReference = instance
        };
        var thread = program.ActiveThread;
        thread.CallStack.Push(callee);
      }
    }

    public static void InvokeAnyCallable(this Program program, object callable, object[] arguments) {
      if (callable is Lambda lambda) {
        program.InvokeLambda(lambda, arguments);
      } else if (callable is Model.Delegate delegation) {
        program.InvokeMethod(delegation.Method, arguments, delegation.Instance, delegation.Closure);
      } else if (callable is LinqExpression linqExpression) {
        program.InvokeLinqExpression(linqExpression, arguments);
      } else {
        throw new NotImplementedException();
      }
    }

    // TODO: unify different types of invocations: lambda, LINQ expressions, delegates, standard methods
    public static void InvokeLinqExpression(this Program program, LinqExpression linqExpression, object[] arguments) {
      var routine = new ExpressionRoutine(linqExpression.Expression, false);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var cause = new Cause($"LINQ clause {linqExpression}", program.ActiveLocation, program.ActiveCause);
      var callee = new Method(null, graph.Entry, linqExpression.Closure, cause);
      if (arguments.Length != 1) {
        throw new NotImplementedException();
      }
      callee.LocalVariables[linqExpression.Parameter].Value = arguments[0];
      if (linqExpression.Closure != null) {
        callee.ThisReference = linqExpression.Closure.ThisReference;
      }
      var thread = program.ActiveThread;
      thread.CallStack.Push(callee);
      program.CheckRecursionBound();
    }

    public static void InvokeLambda(this Program program, Lambda lambda, object[] arguments) {
      var routine = new LambdaRoutine(lambda.Expression);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var lambdaSymbol = (IMethodSymbol)program.CompilationModel.GetReferencedSymbol(lambda.Expression);
      var cause = new Cause($"call {lambdaSymbol}", program.PreviousLocation, program.ActiveCause);
      var callee = new Method(lambdaSymbol, graph.Entry, lambda.Closure, cause);
      callee.PassParameters(lambdaSymbol, arguments);
      if (lambda.Closure != null) {
        callee.ThisReference = lambda.Closure.ThisReference;
      }
      var thread = program.ActiveThread;
      thread.CallStack.Push(callee);
      program.CheckRecursionBound();
    }

    public static void InvokeMethod(this Program program, IMethodSymbol calleeSymbol, object[] arguments, object thisReference, Method closure) {
      if (!calleeSymbol.IsStaticSymbol() && thisReference == null) {
        throw new Model.Exception(program.PreviousLocation, new NullReferenceException());
      }
      SyntaxNode calleeDeclaration = null;
      if (!calleeSymbol.IsAbstract && !calleeSymbol.IsExtern) {
        calleeDeclaration = program.CompilationModel.ResolveSyntaxNode<SyntaxNode>(calleeSymbol);
      }
      if (calleeDeclaration != null) {
        var routine = new MethodRoutine(calleeDeclaration);
        var graph = program.ControlFlowModel.GetGraph(routine);
        var cause = new Cause($"call {calleeSymbol?.Name}", program.PreviousLocation, program.ActiveCause);
        var callee = new Method(calleeSymbol, graph.Entry, closure, cause);
        callee.PassParameters(calleeSymbol, arguments);
        if (!calleeSymbol.IsStaticSymbol()) {
          if (thisReference is Model.Object thisRefObject) {
            callee.ThisReference = thisRefObject;
          } else {
            // TODO: Handle string, Integer etc. as objects
            callee.ThisReference = Unknown.Value;
          }
        }
        var thread = program.ActiveThread;
        thread.CallStack.Push(callee);
        program.CheckRecursionBound();
      } else if (program.CompilationModel.IsRecord(calleeSymbol.ContainingType) && thisReference is Model.Object instance) {
        DeconstructPositionalRecord(program, calleeSymbol, arguments, instance);
      } else {
        if (thisReference == null || thisReference is Model.Object) {
          program.RecordCall(new Call(calleeSymbol.ContainingType, (Model.Object)thisReference));
        }
        program.UnknownCall(calleeSymbol, arguments);
      }
    }

    private static void DeconstructPositionalRecord(Program program, IMethodSymbol calleeSymbol, object[] arguments, Model.Object instance) {
      if (calleeSymbol.Parameters.Length != arguments.Length) {
        throw new NotImplementedException();
      }
      for (int index = 0; index < calleeSymbol.Parameters.Length; index++) {
        var parameter = calleeSymbol.Parameters[index];
        var property = calleeSymbol.ContainingType.GetProperty(parameter.Name);
        if (property != null && arguments[index] is Variable variable) {
          variable.Value = instance.InstanceFields[property].Value;
          program.RecordWrite(new Access(variable, false));
        }
      }
    }

    public static void UnknownCall(this Program program, IMethodSymbol calleeSymbol, object[] arguments) {
      for (int index = 0; index < calleeSymbol.Parameters.Length; index++) {
        if (calleeSymbol.Parameters[index].RefKind == RefKind.Out) {
          if (arguments[index] is Variable variable) {
            variable.Value = Unknown.Value;
          }
        }
      }
      UnknownCall(program, calleeSymbol);
    }

    public static void UnknownCall(this Program program, IMethodSymbol calleeSymbol) {
      if (!calleeSymbol.ReturnsVoid) {
        program.ActiveMethod.EvaluationStack.Push(Unknown.Value);
      }
    }

    public static void IgnoreArguments(this Method caller, int nofParameters) {
      for (int index = 0; index < nofParameters; index++) {
        caller.EvaluationStack.Pop();
      }
    }

    public static void PassSingleParameter(this Thread targetThread, int parameterIndex, object argument) {
      var initialMethod = targetThread.CallStack.Peek();
      var parameter = initialMethod.Symbol.Parameters[parameterIndex].MakeGeneric();
      initialMethod.LocalVariables[parameter].Value = argument;
    }

    public static object[] CollectArguments(this Method caller, IMethodSymbol callee) {
      return CollectArguments(caller, callee.Parameters.Length);
    }

    public static object[] CollectArguments(this Method caller, int nofArguments) {
      var arguments = new object[nofArguments];
      for (int index = nofArguments - 1; index >= 0; index--) {
        arguments[index] = caller.EvaluationStack.Pop();
      }
      return arguments;
    }

    public static void PutBackArguments(this Method caller, object[] arguments) {
      foreach (var argument in arguments) {
        caller.EvaluationStack.Push(argument);
      }
    }

    public static void PassParameters(this Method callee, IMethodSymbol method, object[] arguments) {
      for (int index = 0; index < arguments.Length; index++) {
        var parameter = method.Parameters[index].MakeGeneric();
        var argument = arguments[index];
        if ((parameter.RefKind == RefKind.In || parameter.RefKind == RefKind.Out || parameter.RefKind == RefKind.Ref) && argument is Variable variable) {
          callee.LocalVariables.SetAlias(parameter, variable);
        } else { 
          callee.LocalVariables[parameter].Value = argument;
        }
      }
    }

    public static void CheckRecursionBound(this Program program) {
      var thread = program.ActiveThread;
      var current = thread.CallStack.Peek();
      var recursionDepth =
        (from invocation in thread.CallStack
         where invocation.Symbol != null && invocation.Symbol.Equals(current.Symbol, SymbolEqualityComparer.Default)
         select invocation).Count();
      if (recursionDepth > SimulationBounds.RecursionBound) {
        throw new BoundException("Method recursion bound exceeded");
      }
    }

    private class SyntheticRoutine : Routine {
      public override Location Location {
        get { return Location.None; }
      }

      public override IEnumerable<SyntaxNode> Nodes => throw new NotImplementedException();

      public override bool Equals(object obj) {
        throw new NotImplementedException();
      }

      public override int GetHashCode() {
        throw new NotImplementedException();
      }
    }

    public static void DiscardResult(this Thread thread) {
      var graph = new ControlFlowGraph(new SyntheticRoutine());
      var block = new DiscardBlock(Location.None);
      graph.Entry.Successor = block;
      block.Successor = graph.Exit;
      var callee = new Method(null, graph.Entry, null, new Cause("discard", Location.None));
      thread.CallStack.Push(callee);
    }
  }
}

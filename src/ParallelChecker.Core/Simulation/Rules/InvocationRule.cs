using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class InvocationRule : Rule<InvocationBlock> {
    public override int TimeCost => 10;

    private readonly HashSet<string> specialInvocations = new() {
      Symbols.ThreadStart,
      Symbols.ThreadJoin,
      Symbols.ThreadPoolQueue,
      Symbols.TaskRun,
      Symbols.TaskStart,
      Symbols.TaskWait,
      Symbols.TaskContinueWith,
      Symbols.TaskDelay,
      Symbols.TaskConfigureAwait,
      Symbols.ValueTaskConfigureAwait,
      Symbols.ValueTaskAsTask,
      Symbols.TaskFactoryStartNew,
      Symbols.TaskFactoryContinueWhenAll,
      Symbols.TaskFactoryContinueWhenAny,
      Symbols.TaskWaitAll,
      Symbols.TaskWaitAny,
      Symbols.TaskWhenAll,
      Symbols.TaskWhenAny,
      Symbols.ParallelInvoke,
      Symbols.ParallelFor,
      Symbols.ParallelForEach,
      Symbols.MonitorEnter,
      Symbols.MonitorExit,
      Symbols.MonitorWait,
      Symbols.MonitorPulse,
      Symbols.MonitorPulseAll
    };

    public override bool Applicable(Program program, InvocationBlock block) {
      var method = block.Method;
      if (program.CompilationModel.ContainsSyntaxNode(method)) {
        return true;
      }
      return !method.IsAny(specialInvocations);
    }

    // TODO: Use incremental info for external methods
    public override void Apply(Program program, InvocationBlock block) {
      if (program.InitializeStatics(block.Method.ContainingType)) {
        return;
      }
      var calleeSymbol = block.Method;
      if (!calleeSymbol.IsConstructor() && calleeSymbol.IsInLibrary(program)) {
        program.InvokeLibrary(calleeSymbol);
      } else {
        RegularInvocation(program, block, calleeSymbol);
      }
    }

    private void RegularInvocation(Program program, InvocationBlock block, IMethodSymbol calleeSymbol) {
      var arguments = program.ActiveMethod.CollectArguments(calleeSymbol);
      if (calleeSymbol.MethodKind == MethodKind.DelegateInvoke) {
        InvokeEventOrDelegate(program, calleeSymbol, arguments);
      } else if (calleeSymbol.MethodKind == MethodKind.Constructor) {
        InvokeConstructor(program, calleeSymbol, arguments);
      } else {
        InvokeMethod(program, block.IsVirtual, calleeSymbol, arguments);
      }
    }

    private void InvokeConstructor(Program program, IMethodSymbol calleeSymbol, object[] arguments) {
      var thisReference = (Model.Object)program.ActiveMethod.EvaluationStack.Pop();
      program.GoToNextBlock();
      program.InvokeConstructor(thisReference, calleeSymbol, arguments);
    }

    private void InvokeEventOrDelegate(Program program, IMethodSymbol calleeSymbol, object[] arguments) {
      var value = program.ActiveMethod.EvaluationStack.Pop();
      program.GoToNextBlock();
      if (value == Unknown.Value || value is Thread) {
        // TODO: Suport task cast to Func/Action
        program.UnknownCall(calleeSymbol, arguments);
      } else if (value is Event) {
        InvokeEvent(program, calleeSymbol, arguments, (Event)value);
      } else {
        InvokeDelegateOrLambda(program, calleeSymbol, arguments, value);
      }
    }

    private static void InvokeEvent(Program program, IMethodSymbol calleeSymbol, object[] arguments, Event eventNode) {
      if (eventNode != null) {
        for (int index = eventNode.Handlers.Count - 1; index >= 0; index--) {
          if (!calleeSymbol.ReturnsVoid && index < eventNode.Handlers.Count - 1) {
            program.ActiveThread.DiscardResult();
          }
          var handler = eventNode.Handlers[index];
          InvokeDelegateOrLambda(program, calleeSymbol, arguments, handler);
        }
      }
    }

    private static void InvokeDelegateOrLambda(Program program, IMethodSymbol calleeSymbol, object[] arguments, object handler) {
      if (handler == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      } else if (handler is Model.Delegate delegation) {
        program.InvokeMethod(delegation.Method, arguments, delegation.Instance, delegation.Closure);
      } else if (handler is Lambda lambda) {
        program.InvokeLambda(lambda, arguments);
      } else {
        program.UnknownCall(calleeSymbol, arguments);
      }
    }

    private void InvokeMethod(Program program, bool isVirtual, IMethodSymbol calleeSymbol, object[] arguments) {
      object thisReference = null;
      if (!calleeSymbol.IsStaticSymbol()) {
        // TODO: Homogenize values in simulation. Primitive values, collections etc. should be objects
        thisReference = program.ActiveMethod.EvaluationStack.Pop();
      }
      var dynamicDispatch = isVirtual || calleeSymbol.ContainingType.IsInterface();
      if (dynamicDispatch && (thisReference as Model.Object)?.Type != null) {
        calleeSymbol = ((Model.Object)thisReference).Type.ResolveDynamicDispatch(calleeSymbol);
      }
      var closure = GetLocalFunctionClosure(program, calleeSymbol);
      program.GoToNextBlock();
      program.InvokeMethod(calleeSymbol, arguments, thisReference, closure);
    }

    private Method GetLocalFunctionClosure(Program program, IMethodSymbol calleeSymbol) {
      if (calleeSymbol.ContainingSymbol is IMethodSymbol surroundingMethod) {
        return
          (from activeMehod in program.ActiveThread.CallStack
           from closure in AllClosures(activeMehod)
           where SymbolEqualityComparer.Default.Equals(closure.Symbol.MakeGeneric(), surroundingMethod)
           select closure).First();
      }
      return null;
    }

    private IEnumerable<Method> AllClosures(Method current) {
      if (current != null) {
        yield return current;
        foreach (var result in AllClosures(current.Closure)) {
          yield return result;
        }
      }
    }
  }
}

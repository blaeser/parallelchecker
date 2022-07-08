using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class PropertySetRule : Rule<PropertySetBlock> {
    public override int TimeCost => 5; 

    public override void Apply(Program program, PropertySetBlock block) {
      if (program.InitializeStatics(block.Property.ContainingType)) {
        return;
      }
      if (block.Property.IsInLibrary(false, program)) {
        program.InvokeLibrary(block.Property, false);
      } else {
        RegularPropertySet(program, block);
      }
    }

    private void RegularPropertySet(Program program, PropertySetBlock block) {
      var compilationModel = program.CompilationModel;
      var declaration = compilationModel.ResolveSyntaxNode<SyntaxNode>(block.Property);
      if (declaration == null && !compilationModel.IsPositionalRecordGetter(block.Property) || block.Property.IsExtern || block.Property.IsAbstract) {
        UnknownSetter(program, block.Property);
      } else if (declaration is AnonymousObjectMemberDeclaratorSyntax || compilationModel.IsPositionalRecordGetter(block.Property)) {
        WriteBackingField(program, block);
      } else {
        var property = (BasePropertyDeclarationSyntax)declaration;
        var accessor = property.FindInitOrSetAccesor();
        if (accessor != null && accessor.ExpressionBody != null) {
          var routine = new ExpressionRoutine(accessor.ExpressionBody.Expression, true);
          InvokeSetter(program, block.Property, routine, accessor.ExpressionBody.Expression);
        } else if (accessor == null || accessor.Body == null) {
          WriteBackingField(program, block);
        } else {
          var routine = new PropertyRoutine(accessor);
          InvokeSetter(program, block.Property, routine, accessor.Body);
        }
      }
    }

    private static void WriteBackingField(Program program, PropertySetBlock block) {
      var method = program.ActiveMethod;
      method.IgnoreArguments(block.Property.Parameters.Length);
      if (program.IsDefinedVariable(block.Property)) {
        var variable = program.GetVariable(block.Property);
        var access = new Access(variable, false);
        program.RecordWrite(access);
        variable.Value = method.EvaluationStack.Pop();
      } else {
        program.SkipVariableScope(block.Property);
        method.EvaluationStack.Pop();
      }
      program.GoToNextBlock();
    }

    private static void InvokeSetter(Program program, IPropertySymbol property, Routine routine, SyntaxNode body) {
      var thread = program.ActiveThread;
      var graph = program.ControlFlowModel.GetGraph(routine);
      var cause = new Cause($"access ${property}", program.ActiveLocation, program.ActiveCause);
      var callee = new Method(property.SetMethod, graph.Entry, null, cause);
      PassParameters(program, property, body, callee);
      program.GoToNextBlock();
      thread.CallStack.Push(callee);
      program.CheckRecursionBound();
    }

    private static void PassParameters(Program program, IPropertySymbol property, SyntaxNode body, Method callee) {
      var caller = program.ActiveMethod;
      var arguments = caller.CollectArguments(property.Parameters.Length);
      callee.PassParameters(callee.Symbol, arguments);
      if (!property.IsStaticSymbol()) {
        var thisReference = caller.EvaluationStack.Pop();
        callee.ThisReference = (Model.Object)thisReference ?? throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      var value = caller.EvaluationStack.Pop();
      var parameter = program.CompilationModel.GetReservedValueParameter(body);
      if (parameter != null) {
        callee.LocalVariables[parameter.MakeGeneric()].Value = value;
      }
    }

    private void UnknownSetter(Program program, IPropertySymbol property) {
      var caller = program.ActiveMethod;
      caller.IgnoreArguments(property.Parameters.Length);
      if (!property.IsStaticSymbol()) {
        caller.EvaluationStack.Pop();
      }
      caller.EvaluationStack.Pop();
      program.GoToNextBlock();
    }
  }
}

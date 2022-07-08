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
  internal sealed class PropertyGetRule : Rule<PropertyGetBlock> {
    public override int TimeCost => 5; 

    public override bool Applicable(Program program, PropertyGetBlock block) {
      return !block.Property.Is(Symbols.TaskResult);
    }

    public override void Apply(Program program, PropertyGetBlock block) {
      if (program.InitializeStatics(block.Property.ContainingType)) {
        return;
      }
      if (block.Property.IsInLibrary(true, program)) {
        program.InvokeLibrary(block.Property, true);
      } else {
        RegularPropertyGet(program);
      }
    }

    private void RegularPropertyGet(Program program) {
      var block = (PropertyGetBlock)program.ActiveBlock;
      var compilationModel = program.CompilationModel;
      var declaration = compilationModel.ResolveSyntaxNode<SyntaxNode>(block.Property);
      if (declaration == null && !compilationModel.IsPositionalRecordGetter(block.Property) || block.Property.IsExtern || block.Property.IsAbstract) {
        UnknownGetter(program, block.Property);
      } else if (declaration is AnonymousObjectMemberDeclaratorSyntax || compilationModel.IsPositionalRecordGetter(block.Property)) {
        ReadBackingField(program, block);
      } else if ((declaration as PropertyDeclarationSyntax)?.ExpressionBody != null) {
        var property = (PropertyDeclarationSyntax)declaration;
        var routine = new ExpressionRoutine(property.ExpressionBody.Expression, false);
        InvokeGetter(program, block.Property, routine);
      } else if ((declaration as IndexerDeclarationSyntax)?.ExpressionBody != null) {
        var indexer = (IndexerDeclarationSyntax)declaration;
        var routine = new ExpressionRoutine(indexer.ExpressionBody.Expression, false);
        InvokeGetter(program, block.Property, routine);
      } else {
        var property = (BasePropertyDeclarationSyntax)declaration;
        var accessor = property.FindGetAccessor();
        if (accessor.ExpressionBody != null) {
          var routine = new ExpressionRoutine(accessor.ExpressionBody.Expression, false);
          InvokeGetter(program, block.Property, routine);
        } else if (accessor.Body == null) {
          ReadBackingField(program, block);
        } else {
          var routine = new PropertyRoutine(accessor);
          InvokeGetter(program, block.Property, routine);
        }
      }
    }

    private static void ReadBackingField(Program program, PropertyGetBlock block) {
      var method = program.ActiveMethod;
      method.IgnoreArguments(block.Property.Parameters.Length);
      if (program.IsDefinedVariable(block.Property)) {
        var variable = program.GetVariable(block.Property);
        var access = new Access(variable, false);
        program.RecordRead(access);
        method.EvaluationStack.Push(variable.Value);
      } else {
        program.SkipVariableScope(block.Property);
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }

    private static void InvokeGetter(Program program, IPropertySymbol property, Routine routine) {
      var thread = program.ActiveThread;
      var graph = program.ControlFlowModel.GetGraph(routine);
      var cause = new Cause($"access {property}", program.ActiveLocation, program.ActiveCause);
      var callee = new Method(property.GetMethod, graph.Entry, null, cause);
      PassParameters(program, property, callee);
      program.GoToNextBlock();
      thread.CallStack.Push(callee);
      program.CheckRecursionBound();
    }

    private static void PassParameters(Program program, IPropertySymbol property, Method callee) {
      var caller = program.ActiveMethod;
      var arguments = caller.CollectArguments(callee.Symbol);
      callee.PassParameters(callee.Symbol, arguments);
      if (!property.IsStaticSymbol()) {
        var thisReference = caller.EvaluationStack.Pop();
        if (thisReference == null) {
          throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
        }
        if (thisReference is Model.Object thisRefObject) {
          callee.ThisReference = thisRefObject;
        } else {
          // TODO: Handle string, Integer etc. as objects
          callee.ThisReference = Unknown.Value;
        }
      }
    }

    private void UnknownGetter(Program program, IPropertySymbol property) {
      var caller = program.ActiveMethod;
      caller.IgnoreArguments(property.Parameters.Length);
      if (!property.IsStaticSymbol()) {
        caller.EvaluationStack.Pop();
      }
      caller.EvaluationStack.Push(Unknown.Value);
      program.GoToNextBlock();
    }
  }
}

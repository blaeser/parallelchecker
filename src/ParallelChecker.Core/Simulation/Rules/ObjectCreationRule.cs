using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ObjectCreationRule : Rule<ObjectCreationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, ObjectCreationBlock block) {
      var type = block.Type;
      if (program.CompilationModel.ContainsSyntaxNode(type)) {
        return true;
      }
      return type == null || !type.IsAny(Symbols.Thread, Symbols.Task);
    }

    public override void Apply(Program program, ObjectCreationBlock block) {
      if (program.InitializeStatics(block.Type)) {
        return;
      }
      if (block.Constructor.IsInLibrary(program)) {
        program.InvokeLibrary(block.Constructor);
      } else {
        RegularObjectCreation(program, block);
      }
    }

    private static void RegularObjectCreation(Program program, ObjectCreationBlock block) {
      var type = block.Type;
      if (type?.TypeKind == TypeKind.Delegate) {
        program.GoToNextBlock();
        return;
      }
      var caller = program.ActiveMethod;
      int size = 1;
      if (type != null) {
        size += type.GetInstanceFields().Count();
      }
      program.IncreaseHeapSize(size);
      var instance = new Object(type);
      if (type != null && type.HasDestructor()) {
        program.Finalizeables.Add(instance);
      }
      if (block.Constructor != null) {
        var arguments = caller.CollectArguments(block.Constructor);
        caller.EvaluationStack.Push(instance);
        program.GoToNextBlock();
        program.InvokeConstructor(instance, block.Constructor, arguments);
      } else {
        caller.IgnoreArguments(block.NofParameters);
        caller.EvaluationStack.Push(instance);
        program.GoToNextBlock();
      }
    }
  }
}

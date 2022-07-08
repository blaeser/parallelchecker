using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ThreadCreationRule : Rule<ObjectCreationBlock> {
    public override int TimeCost => 10;

    public override bool Applicable(Program program, ObjectCreationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Type) ||
          program.CompilationModel.ContainsSyntaxNode(block.Constructor)) {
        return false;
      }
      // TODO: Consider separating thread and task creations
      return block.Type.IsAny(Symbols.Thread, Symbols.Task);
    }

    public override void Apply(Program program, ObjectCreationBlock block) {
      // TODO: Consider various thread and task constructors
      var method = program.ActiveMethod;
      var arguments = method.CollectArguments(block.NofParameters);
      var threadStart = arguments[0];
      if (program.IsThreadStart(threadStart)) {
        var newThread = program.CreateThread(threadStart);
        method.EvaluationStack.Push(newThread);
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }
  }
}

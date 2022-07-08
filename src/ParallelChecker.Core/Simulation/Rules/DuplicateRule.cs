using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class DuplicateRule : Rule<DuplicateBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, DuplicateBlock block) {
      var method = program.ActiveMethod;
      var stack = method.EvaluationStack;
      stack.Push(stack.Peek());
      program.GoToNextBlock();
    }
  }
}

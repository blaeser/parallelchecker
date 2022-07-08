using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class SwapRule : Rule<SwapBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, SwapBlock block) {
      var method = program.ActiveMethod;
      var first = method.EvaluationStack.Pop();
      var second = method.EvaluationStack.Pop();
      method.EvaluationStack.Push(first);
      method.EvaluationStack.Push(second);
      program.GoToNextBlock();
    }
  }
}

using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class UnknownRule : Rule<UnknownBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, UnknownBlock block) {
      var method = program.ActiveMethod;
      method.EvaluationStack.Push(Unknown.Value);
      program.GoToNextBlock();
    }
  }
}

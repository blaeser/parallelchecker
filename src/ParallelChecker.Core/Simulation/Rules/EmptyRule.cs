using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class EmptyRule : Rule<EmptyBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, EmptyBlock block) {
      program.GoToNextBlock();
    }
  }
}

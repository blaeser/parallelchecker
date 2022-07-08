using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class EntryRule : Rule<EntryBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, EntryBlock block) {
      program.GoToNextBlock();
    }
  }
}

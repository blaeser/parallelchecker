using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ForeachEndRule : Rule<IteratorEndBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, IteratorEndBlock block) {
      var method = program.ActiveMethod;
      method.OpenIterators.Pop();
      program.GoToNextBlock();
    }
  }
}

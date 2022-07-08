using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class UndeclareRule : Rule<UndeclareBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, UndeclareBlock block) {
      var method = program.ActiveMethod;
      method.LocalVariables.Remove(block.Variable);
      program.GoToNextBlock();
    }
  }
}

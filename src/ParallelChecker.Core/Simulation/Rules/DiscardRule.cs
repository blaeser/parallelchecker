using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class DiscardRule : Rule<DiscardBlock> {
    public override int TimeCost => 1; 

    public override void Apply(Program program, DiscardBlock block) {
      var method = program.ActiveMethod;
      var value = method.EvaluationStack.Pop();
      if (value is Object) {
        program.CollectGarbage();
      }
      program.GoToNextBlock();
    }
  }
}

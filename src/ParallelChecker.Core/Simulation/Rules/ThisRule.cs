using ParallelChecker.Core.Simulation.Model;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;

namespace ParallelChecker.Core.Simulation.Rules {
  internal class ThisRule : Rule<ThisBlock> {
    public override int TimeCost => 1;

    public override void Apply(Program program, ThisBlock block) {
      var method = program.ActiveMethod;
      method.EvaluationStack.Push(method.ThisReference);
      program.GoToNextBlock();
    }
  }
}

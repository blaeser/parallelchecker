using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class EnterTryRule : Rule<EnterTryBlock> {
    public override int TimeCost => 2;

    public override void Apply(Program program, EnterTryBlock block) {
      var method = program.ActiveMethod;
      var handler = new ExceptionHandler(block, method.EvaluationStack.Count);
      method.OpenExceptionHandlers.Push(handler);
      program.GoToNextBlock();
    }
  }
}

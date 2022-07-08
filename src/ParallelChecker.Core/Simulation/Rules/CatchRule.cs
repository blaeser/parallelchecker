using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class CatchRule : Rule<CatchBlock> {
    public override int TimeCost => 10; 

    public override void Apply(Program program, CatchBlock block) {
      var method = program.ActiveMethod;
      var handler = method.OpenExceptionHandlers.Peek();
      handler.UnhandledException = null;
      program.GoToNextBlock();
    }
  }
}

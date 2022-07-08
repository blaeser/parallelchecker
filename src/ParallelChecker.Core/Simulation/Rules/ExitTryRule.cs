using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ExitTryRule : Rule<ExitTryBlock> {
    public override int TimeCost => 10; 

    public override void Apply(Program program, ExitTryBlock block) {
      var method = program.ActiveMethod;
      var handler = method.OpenExceptionHandlers.Peek();
      switch (handler.State) {
        case ExceptionHandlerState.Try:
          handler.ResumeAfterFinally = block.Successor;
          program.GotoFinally();
          break;
        case ExceptionHandlerState.Catch:
          program.GotoFinally();
          break;
        case ExceptionHandlerState.Finally:
          program.FinallyExit();
          break;
        default:
          throw new NotImplementedException();
      }
    }
  }
}

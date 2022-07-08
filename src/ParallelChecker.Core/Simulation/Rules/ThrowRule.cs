using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ThrowRule : Rule<ThrowBlock> {
    public override int TimeCost => 10;

    public override void Apply(Program program, ThrowBlock block) {
      var method = program.ActiveMethod;
      Model.Object exception;
      if (block.IsRethrow) {
        var handler = method.OpenExceptionHandlers.Peek();
        exception = handler.UnhandledException.Instance;
      } else {
        exception = (Model.Object)method.EvaluationStack.Pop();
      }
      if (exception == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      throw new Model.Exception(program.ActiveLocation, exception);
    }
  }
}

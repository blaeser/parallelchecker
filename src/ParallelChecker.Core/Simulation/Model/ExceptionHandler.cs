using ParallelChecker.Core.ControlFlow.Blocks;
using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal enum ExceptionHandlerState { Try, Catch, Finally }

  internal sealed class ExceptionHandler {
    public EnterTryBlock Block { get; }
    public int EvaluationStackSize { get; }

    public ExceptionHandlerState State { get; set; } = ExceptionHandlerState.Try;
    public Exception UnhandledException { get; set; }
    public BasicBlock ResumeAfterFinally { get; set; }

    public ExceptionHandler(EnterTryBlock block, int evaluationStackSize) {
      if (evaluationStackSize < 0) {
        throw new ArgumentException(nameof(evaluationStackSize));
      }
      Block = block ?? throw new ArgumentNullException(nameof(block));
      EvaluationStackSize = evaluationStackSize;
    }
  }
}

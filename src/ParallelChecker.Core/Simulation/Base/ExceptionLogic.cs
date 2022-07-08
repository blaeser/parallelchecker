using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class ExceptionLogic {
    public static void HandleException(this Program program, Model.Exception exception) {
      while (program.ActiveThread.CallStack.Count > 0) {
        var method = program.ActiveMethod;
        while (method.OpenExceptionHandlers.Count > 0) {
          try {
            ExecuteHandler(program, exception, method);
            return;
          } catch (Model.Exception latest) {
            exception = latest;
          }
        }
        program.ActiveThread.CallStack.Pop();
      }
      program.TerminateThread();
    }

    private static void ExecuteHandler(Program program, Model.Exception exception, Method method) {
      var handler = method.OpenExceptionHandlers.Peek();
      CleanupEvaluationStack(method, handler.EvaluationStackSize);
      handler.UnhandledException = exception;
      switch (handler.State) {
        case ExceptionHandlerState.Try:
          handler.State = ExceptionHandlerState.Catch;
          method.EvaluationStack.Push(exception.Instance);
          program.GoToNextBlock(handler.Block.Catches);
          return;
        case ExceptionHandlerState.Catch:
          program.GotoFinally();
          return;
        case ExceptionHandlerState.Finally:
          program.FinallyExit();
          return;
        default:
          throw new NotImplementedException();
      }
    }

    private static void CleanupEvaluationStack(Method method, int evaluationStackSize) {
      if (method.EvaluationStack.Count < evaluationStackSize) {
        method.EvaluationStack.Push(Unknown.Value);
      }
      while (method.EvaluationStack.Count > evaluationStackSize) {
        method.EvaluationStack.Pop();
      }
    }

    public static void GotoFinally(this Program program) {
      var method = program.ActiveMethod;
      var handler = method.OpenExceptionHandlers.Peek();
      handler.State = ExceptionHandlerState.Finally;
      program.GoToNextBlock(handler.Block.Finally);
    }

    public static void FinallyExit(this Program program) {
      var method = program.ActiveMethod;
      var handler = method.OpenExceptionHandlers.Pop();
      if (handler.UnhandledException != null) {
        throw handler.UnhandledException;
      }
      if (handler.ResumeAfterFinally != null) {
        program.GoToNextBlock(handler.ResumeAfterFinally);
      } else {
        program.GoToNextBlock();
      }
    }
  }
}

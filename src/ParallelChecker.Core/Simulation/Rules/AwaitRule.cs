using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class AwaitRule : Rule<AwaitBlock> {
    public override int TimeCost => 2;

    public override void Apply(Program program, AwaitBlock block) {
      var currentThread = program.ActiveThread;
      var method = program.ActiveMethod;
      var value = method.EvaluationStack.Peek();
      if (value == Unknown.Value) {
        currentThread.State = ThreadState.Waiting;
        return;
      } else if (method.EvaluationStack.Peek() == null) {
        throw new Exception(program.ActiveLocation, new System.NullReferenceException());
      } else if (method.EvaluationStack.Peek() is Thread otherThread) {
        if (program.JoinThread(otherThread, true)) {
          ReturnNonBlocking(program, otherThread.Result);
        } else {
          ReturnFromBlockingAwait(program, otherThread.ConfigureAwait);
        }
      } else {
        ReturnNonBlocking(program, Unknown.Value);
      }
    }

    private static void ReturnNonBlocking(Program program, object value) {
      var method = program.ActiveMethod;
      method.EvaluationStack.Pop();
      method.EvaluationStack.Push(value);
      program.GoToNextBlock();
    }

    private void ReturnFromBlockingAwait(Program program, bool configureAwait) {
      var currentThread = program.ActiveThread;
      if (currentThread.CallStack.Count > 1) {
        // TODO: Deregister from otherThreads.WaitersForJoin
        currentThread.State = ThreadState.Runnable;
        var method = currentThread.ActiveMethod;
        var cause = new Cause("await continuation", program.ActiveLocation, program.ActiveCause);
        var newThread = new Thread(program, cause, method);
        program.StartThread(newThread);
        if (configureAwait && currentThread.Dispatcher != null) {
          currentThread.Dispatcher.Register(newThread);
        } 
        AsyncReturn(program, newThread);
      } else if (currentThread.Dispatcher != null) {
        currentThread.Dispatcher.PauseCurrent();
      }
    }

    private void AsyncReturn(Program program, object result) {
      var thread = program.ActiveThread;
      var method = thread.ActiveMethod;
      thread.CallStack.Pop();
      if (!method.Symbol.ReturnsVoid) {
        var caller = thread.ActiveMethod;
        caller.EvaluationStack.Push(result);
      }
    }
  }
}

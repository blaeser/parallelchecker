using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class ExitRule : Rule<ExitBlock> {
    public override int TimeCost => 3; 

    public override void Apply(Program program, ExitBlock block) {
      var thread = program.ActiveThread;
      var method = thread.ActiveMethod;
      thread.CallStack.Pop();
      if (method.Symbol != null && !method.Symbol.ReturnsVoid) {
        if (method.Symbol.IsAsync) {
          PrepareAsyncResult(program, thread, method);
        }
        ReturnValue(thread, method);
      } else if (method.Symbol == null && method.EvaluationStack.Count() > 0) { 
        // Task.WhenAny or LINQ expression clause
        ReturnValue(thread, method);
      }
      if (method.EvaluationStack.Count != 0) {
        throw new System.Exception();
      }
      if (thread.CallStack.Count == 0) {
        program.TerminateThread();
      }
      if (ContainsReferences(method)) {
        program.CollectGarbage();
      }
    }

    private bool ContainsReferences(Method method) {
      return
        (from variable in method.LocalVariables.AllVariables()
         let value = variable.Value
         where value is Object
         select value).Any();
    }

    private static void PrepareAsyncResult(Program program, Thread thread, Method method) {
      object value = null;
      if (method.EvaluationStack.Count > 0) {
        value = method.EvaluationStack.Pop();
      }
      if (thread.CallStack.Count > 0 && value is not Thread) {
        var task = new Thread(program, thread.Cause, method) {
          State = ThreadState.Terminated,
          Result = value
        };
        value = task;
      }
      method.EvaluationStack.Push(value);
    }

    private static void ReturnValue(Thread thread, Method method) {
      var value = method.EvaluationStack.Pop();
      if (method.ResultInterceptor != null) {
        method.ResultInterceptor(value);
        return;
      }
      if (thread.CallStack.Count > 0) {
        var caller = thread.ActiveMethod;
        caller.EvaluationStack.Push(value);
      } else {
        thread.Result = value;
      }
    }
  }
}

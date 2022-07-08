using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Rules {
  internal class TaskMultiWaitRule : Rule<InvocationBlock> {
    public override int TimeCost => 6;

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      return block.Method.IsAny(Symbols.TaskWaitAll, Symbols.TaskWaitAny);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var waitAll = block.Method.Is(Symbols.TaskWaitAll);
      var currentThread = program.ActiveThread;
      var method = currentThread.ActiveMethod;

      // TODO currently the varargs transformation creates nested arrays if the vararg parameter is supplied with an array.
      // TODO "randomized" wait timeouts when a timeout is supplied?
      var arguments = method.CollectArguments(block.Method);
      var otherThreads = ExtractThreads(arguments[0]);

      if (otherThreads != null) {
        var threadIsWaiting = false;
        if (waitAll) {
          threadIsWaiting = !otherThreads.All(otherThread => program.JoinThread(otherThread, true));
          if (!threadIsWaiting && !block.Method.ReturnsVoid) {
            method.EvaluationStack.Push(false);
          }
        } else {
          var joinedThreadIndex = otherThreads.FindIndex(otherThread => program.JoinThread(otherThread, true));
          threadIsWaiting = joinedThreadIndex == -1;
          if (!threadIsWaiting) {
            method.EvaluationStack.Push(joinedThreadIndex);
            currentThread.State = ThreadState.Runnable;
          }
        }

        if (threadIsWaiting) {
          method.PutBackArguments(arguments);
        } else {
          program.GoToNextBlock();
        }
      } else {
        program.UnknownCall(block.Method);
        program.GoToNextBlock();
      }
    }

    private List<Thread> ExtractThreads(object value) {
      if (value is not Array array) {
        return null;
      }
      var result = new List<Thread>();
      foreach (var item in array.AllValues()) {
        if (item is Thread thread) {
          result.Add(thread);
        } else {
          return null;
        }
      }
      return result;
    }
  }
}

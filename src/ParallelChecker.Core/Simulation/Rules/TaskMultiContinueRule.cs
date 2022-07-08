using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Blocks;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Library;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Rules {
  internal sealed class TaskMultiContinueRule : Rule<InvocationBlock> {
    public override int TimeCost => 3;

    private static readonly HashSet<string> continuations = new() {
      Symbols.TaskFactoryContinueWhenAll,
      Symbols.TaskFactoryContinueWhenAny,
      Symbols.TaskWhenAll,
      Symbols.TaskWhenAny
    };

    public override bool Applicable(Program program, InvocationBlock block) {
      if (program.CompilationModel.ContainsSyntaxNode(block.Method)) {
        return false;
      }
      var method = block.Method;
      return method.IsAny(continuations);
    }

    public override void Apply(Program program, InvocationBlock block) {
      var isCombinedTask = block.Method.Is(Symbols.TaskWhenAll) || block.Method.Is(Symbols.TaskWhenAny);
      var whenAll = block.Method.Is(Symbols.TaskFactoryContinueWhenAll) || block.Method.Is(Symbols.TaskWhenAll);
      var currentThread = program.ActiveThread;
      var method = program.ActiveMethod;
      int nofOtherArgs = isCombinedTask ? 1 : 2;
      var otherArguments = method.CollectArguments(block.Method.Parameters.Length - nofOtherArgs);
      object successor = null;
      if (!isCombinedTask) {
        successor = method.EvaluationStack.Pop();
      }
      var predecessors = ExtractThreads(method.EvaluationStack.Pop());
      if (!isCombinedTask) {
        method.EvaluationStack.Pop(); // pop Task.Factory
      }
      var scheduler = otherArguments.FirstOfType<TaskScheduler>();
      if (predecessors != null && (isCombinedTask || program.IsThreadStart(successor))) {
        Thread nextThread;
        if (isCombinedTask) {
          nextThread = CreateCombinedTask(program, whenAll);
        } else {
          nextThread = program.CreateThread(successor);
        }
        currentThread.AdvanceTime();
        nextThread.SynchronizeWith(currentThread);
        nextThread.Dispatcher = scheduler?.Dispatcher;
        var waitState = new MultiTaskWaitState();
        foreach (var previousThread in predecessors) {
          previousThread.Continuations.Add(nextThread);
          waitState.Predecessors.Add(previousThread);
        }
        if (whenAll) {
          nextThread.WaitState = waitState;
        }
        foreach (var previousThread in predecessors) {
          if (previousThread.State == ThreadState.Terminated) {
            program.RunContinuation(previousThread, nextThread);
          }
        }
        method.EvaluationStack.Push(nextThread);
      } else {
        method.EvaluationStack.Push(Unknown.Value);
      }
      program.GoToNextBlock();
    }

    private Thread CreateCombinedTask(Program program, bool whenAll) {
      var cause = new Cause(whenAll ? "Task.WhenAll" : "Task.WhenAny", program.ActiveLocation);
      var method = new Method(null, new ExitBlock(Location.None), null, cause);
      return new Thread(program, cause, method);
    }

    private List<Thread> ExtractThreads(object value) {
      if (value is not Model.Array array) {
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

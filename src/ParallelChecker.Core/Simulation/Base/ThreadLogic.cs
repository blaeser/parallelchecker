using Microsoft.CodeAnalysis;
using ParallelChecker.Core.ControlFlow.Routines;
using ParallelChecker.Core.General;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class ThreadLogic {
    public static bool IsThreadStart(this Program program, object input) {
      return 
        input is Lambda || 
        input is Model.Delegate delegation && 
          program.CompilationModel.ResolveSyntaxNode<SyntaxNode>(delegation.Method.MakeGeneric()).IsMethod() || 
        input is LinqExpression;
    }

    public static Thread CreateThread(this Program program, object threadStart) {
      if (threadStart == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      } else if (threadStart is Lambda lambda) {
        return program.CreateThread(lambda);
      } else if (threadStart is Model.Delegate delegation) {
        return program.CreateThread(delegation);
      } else if (threadStart is LinqExpression linqExpression) {
        return program.CreateThread(linqExpression);
      } else {
        throw new NotImplementedException();
      }
    }

    // TODO: unify different starts: lambda, delegate, LINQ expression together with method calls InvocationLogic 
    private static Thread CreateThread(this Program program, LinqExpression linqExpression) {
      var routine = new ExpressionRoutine(linqExpression.Expression, false);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var cause = new Cause("LINQ clause", linqExpression.Expression.GetLocation(), program.ActiveCause);
      var initialMethod = new Method(null, graph.Entry, linqExpression.Closure, cause);
      if (linqExpression.Closure != null) {
        initialMethod.ThisReference = linqExpression.Closure.ThisReference;
      }
      return new Thread(program, cause, initialMethod);
    }

    private static Thread CreateThread(this Program program, Lambda lambda) {
      var routine = new LambdaRoutine(lambda.Expression);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var lambdaSymbol = (IMethodSymbol)program.CompilationModel.GetReferencedSymbol(lambda.Expression);
      var cause = new Cause("thread or task", lambda.Expression.GetLocation(), program.ActiveCause);
      var initialMethod = new Method(lambdaSymbol, graph.Entry, lambda.Closure, cause);
      if (lambda.Closure != null) {
        initialMethod.ThisReference = lambda.Closure.ThisReference;
      }
      return new Thread(program, cause, initialMethod);
    }

    private static Thread CreateThread(this Program program, Model.Delegate delegation) {
      var methodDeclaration = program.CompilationModel.ResolveSyntaxNode<SyntaxNode>(delegation.Method.MakeGeneric());
      if (!methodDeclaration.IsMethod()) {
        throw new System.Exception("Unresolved delegate method");
      }
      var routine = new MethodRoutine(methodDeclaration);
      var graph = program.ControlFlowModel.GetGraph(routine);
      var cause = new Cause("thread or task", methodDeclaration.MethodIdentifier().GetLocation(), program.ActiveCause);
      var initialMethod = new Method(delegation.Method, graph.Entry, delegation.Closure, cause) {
        ThisReference = delegation.Instance
      };
      return new Thread(program, cause, initialMethod);
    }

    public static void StartThread(this Program program, Thread otherThread) {
      if (otherThread == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      var currentThread = program.ActiveThread;
      currentThread.AdvanceTime();
      otherThread.SynchronizeWith(currentThread);
      otherThread.State = ThreadState.Runnable;
      program.AllThreads.Add(otherThread);
      if (program.AllThreads.Count > SimulationBounds.ThreadAmountBound) {
        throw new BoundException("Thread amount bound exceeded");
      }
    }

    public static bool JoinThread(this Program program, Thread otherThread, bool isTaskWait) {
      if (otherThread == null) {
        throw new Model.Exception(program.ActiveLocation, new NullReferenceException());
      }
      var currentThread = program.ActiveThread;
      if (otherThread.State != ThreadState.Terminated && (isTaskWait || otherThread.State != ThreadState.Created)) {
        currentThread.State = ThreadState.Waiting;
        otherThread.WaitersForJoin.Enqueue(currentThread);
        currentThread.WaitingOn = otherThread;
        DeadlockMonitor.CheckForDeadlock(program);
        return false;
      } else {
        currentThread.SynchronizeWith(otherThread);
        currentThread.AdvanceTime();
        return true;
      }
    }

    public static void TerminateThread(this Program program) {
      var thread = program.ActiveThread;
      thread.AdvanceTime();
      thread.State = ThreadState.Terminated;
      SyncFromStaticLoader(program, thread);
      thread.WaitersForJoin.NotifyAll();
      thread.Dispatcher?.ScheduleNext();
      foreach (var continuation in thread.Continuations) {
        RunContinuation(program, thread, continuation);
      }
      if (thread.Timer != null && thread.Timer.Active) {
        thread.Timer.StartTimer(program);
      }
    }

    private static void SyncFromStaticLoader(this Program program, Thread thread) {
      if (thread == program.StaticLoader) {
        foreach (var other in program.AllThreads) {
          if (other.SyncWithStatic && other.State != ThreadState.Terminated) {
            other.SynchronizeWith(program.StaticLoader);
          }
        }
      }
    }

    public static void RunContinuation(this Program program, Thread preceding, Thread succeeding) {
      if (IsContinuationReady(succeeding) && succeeding.State == ThreadState.Created) {
        SynchronizeContinuation(preceding, succeeding);
        var dispatcher = succeeding.Dispatcher;
        succeeding.Dispatcher = null;
        program.StartThread(succeeding);
        var target = succeeding.ActiveMethod;
        if (target.Symbol == null) {
          PassTaskCombinatorArgument(preceding, succeeding, target);
        } else {
          PassContinuationArguments(program, preceding, succeeding, target);
        }
        dispatcher?.Register(succeeding);
      }
    }

    private static void PassTaskCombinatorArgument(Thread preceding, Thread succeeding, Method target) {
      if (succeeding.WaitState is not MultiTaskWaitState) {
        // Task.WhenAny result, codesigned with TaskMultiContinueRule
        target.EvaluationStack.Push(preceding);
      }
    }

    private static void PassContinuationArguments(Program program, Thread preceding, Thread succeeding, Method target) {
      var parameters = target.Symbol.Parameters;
      var arguments = new object[parameters.Length];
      if (succeeding.WaitState is MultiTaskWaitState state) {
        var values = state.Predecessors.Cast<object>().ToList();
        arguments[0] = new Model.Array(values, program);
        succeeding.WaitState = null;
      } else {
        arguments[0] = preceding;
      }
      for (int index = 1; index < parameters.Length; index++) {
        arguments[index] = Unknown.Value;
      }
      target.PassParameters(target.Symbol, arguments);
    }

    private static bool IsContinuationReady(Thread continuation) {
      if (continuation.State != ThreadState.Created) {
        return false;
      }
      if (continuation.WaitState is MultiTaskWaitState state) {
        return
          (from predecessor in state.Predecessors
           where predecessor.State != ThreadState.Terminated
           select predecessor).Count() == 0;
      }
      return true;
    }

    private static void SynchronizeContinuation(Thread singlePreceding, Thread successor) {
      if (successor.WaitState is MultiTaskWaitState state) {
        foreach (var predecessor in state.Predecessors) {
          successor.SynchronizeWith(predecessor);
        }
      } else {
        successor.SynchronizeWith(singlePreceding);
      }
    }

    public static void NotifyAll(this Queue<Thread> queue) {
      while (queue.Count > 0) {
        queue.NotifySingle();
      }
    }

    public static void NotifySingle(this Queue<Thread> queue) {
      if (queue.Count > 0) {
        var thread = queue.Dequeue();
        thread.WaitingOn = null;
        if (thread.State == ThreadState.Waiting) {
          if (thread.Dispatcher == null) {
            thread.State = ThreadState.Runnable;
          } else {
            thread.Dispatcher.ScheduleNext();
          }
        }
      }
    }
  }
}

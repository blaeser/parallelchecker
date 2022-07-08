using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  internal static class LockExtensions {
    public static void Wait(this Program program, Object instance, Queue<Thread> waiting) {
      program.RecordVariations();
      var thread = program.ActiveThread;
      thread.State = ThreadState.Waiting;
      waiting.Enqueue(thread);
      thread.WaitingOn = instance;
      program.CheckForDeadlock();
    }

    public static void Pass(this Program program, VectorTime unlockTime) {
      var thread = program.ActiveThread;
      thread.Time.SynchronizeWith(unlockTime);
      thread.AdvanceTime();
    }

    public static void SignalAll(this Program program, Queue<Thread> waiting, VectorTime unlockTime) {
      waiting.NotifyAll();
      Exit(program, unlockTime);
    }

    public static void SignalSingle(this Program program, Queue<Thread> waiting, VectorTime unlockTime) {
      waiting.NotifySingle();
      Exit(program, unlockTime);
    }

    private static void Exit(Program program, VectorTime unlockTime) {
      var thread = program.ActiveThread;
      thread.AdvanceTime();
      unlockTime.SynchronizeWith(thread.Time);
      thread.AdvanceTime();
    }
  }
}

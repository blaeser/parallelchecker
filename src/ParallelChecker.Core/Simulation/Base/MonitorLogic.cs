using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Base {
  internal static class MonitorLogic {
    public static bool TryMonitorEnter(this Program program, Object target) {
      var thread = program.ActiveThread;
      program.RecordVariations();
      if (target == Unknown.Value) {
        thread.State = ThreadState.Waiting;
        return false;
      } else if (target.LockHolders.Count > 0 && !target.LockHolders.Contains(thread)) {
        thread.State = ThreadState.Waiting;
        target.LockWaiters.Enqueue(thread);
        thread.WaitingOn = target;
        program.CheckForDeadlock();
        return false;
      } else {
        thread.NestedLocks++;
        target.LockCounter++;
        if (target.LockHolders.Count == 0) {
          target.LockHolders.Add(thread);
          thread.Time.SynchronizeWith(target.UnlockTime);
          thread.AdvanceTime();
        }
        program.GoToNextBlock();
        return true;
      }
    }

    public static void MonitorExit(this Program program, Object target) {
      var thread = program.ActiveThread;
      program.RecordVariations();
      if (target != Unknown.Value) {
        target.LockCounter--;
        if (target.LockCounter == 0) {
          thread.NestedLocks--;
          target.LockHolders.Remove(thread);
          thread.AdvanceTime();
          target.UnlockTime.SynchronizeWith(thread.Time);
          target.LockWaiters.NotifySingle();
          thread.AdvanceTime();
        }
      }
      program.GoToNextBlock();
    }
  }
}

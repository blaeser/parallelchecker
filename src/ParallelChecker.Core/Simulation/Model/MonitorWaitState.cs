using System;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class MonitorWaitState : WaitState {
    public Object LockTarget { get; }
    public int LockCount { get; }

    public MonitorWaitState(Object lockTarget, int lockCount) {
      LockTarget = lockTarget ?? throw new ArgumentNullException(nameof(lockTarget));
      if (lockCount <= 0) {
        throw new ArgumentNullException(nameof(lockCount));
      }
      LockCount = lockCount;
    }
  }
}

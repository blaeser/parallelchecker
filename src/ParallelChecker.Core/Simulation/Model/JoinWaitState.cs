using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class JoinWaitState : WaitState {
    public HashSet<Thread> AwaitedThreads { get; } = new();
  }
}

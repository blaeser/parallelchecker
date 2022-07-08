using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class MultiTaskWaitState : WaitState {
    public List<Thread> Predecessors { get; } = new();
  }
}

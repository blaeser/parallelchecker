using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Event {
    public List<object> Handlers { get; } = new();
  }
}

using System;

namespace ParallelChecker.Core.Simulation {
  [Serializable]
  internal class BoundException : Exception {
    public BoundException(string message) :
      base(message) {
    }
  }
}

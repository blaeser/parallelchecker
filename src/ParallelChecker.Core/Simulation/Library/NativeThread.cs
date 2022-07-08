using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading", "Thread")]
  internal sealed class NativeThread {
    [Member]
    public static Thread GetCurrentThread(Program program) {
      return program.ActiveThread;
    }
  }
}

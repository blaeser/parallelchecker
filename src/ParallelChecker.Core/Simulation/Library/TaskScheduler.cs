using ParallelChecker.Core.Simulation.Model;
using System;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading.Tasks")]
  class TaskScheduler : Model.Object {
    public Dispatcher Dispatcher { get; }

    private TaskScheduler(Dispatcher dispatcher) {
      Dispatcher = dispatcher;
    }

    [Member]
    public static TaskScheduler FromCurrentSynchronizationContext(Program program) {
      return new TaskScheduler(program.ActiveThread.Dispatcher);
    }
  }
}

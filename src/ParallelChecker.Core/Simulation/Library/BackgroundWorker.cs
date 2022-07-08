using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.ComponentModel")]
  internal sealed class BackgroundWorker : Object {
    [Member]
    public readonly Event DoWork = new();

    [Member]
    public BackgroundWorker() {
    }

    [Member]
    public void RunWorkerAsync(Program program) {
      foreach (var handler in DoWork.Handlers) {
        if (program.IsThreadStart(handler)) {
          var newThread = program.CreateThread(handler);
          program.StartThread(newThread);
        }
      }
    }

    // TODO: Support other features, on completion, progress, cancellation etc.
  }
}

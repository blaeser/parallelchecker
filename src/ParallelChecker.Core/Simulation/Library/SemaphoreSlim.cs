using ParallelChecker.Core.Simulation.Model;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class SemaphoreSlim : Object {
    private readonly Semaphore _semaphore;

    [Member]
    public SemaphoreSlim(int count) : 
      base(typeof(System.Threading.SemaphoreSlim)) {
      _semaphore = new Semaphore(count, int.MaxValue);
    }

    [Member]
    public void Wait(Program program) {
      _semaphore.WaitOne(program);
    }

    [Member]
    public int Release(Program program, int releaseCount) {
      return _semaphore.Release(program, releaseCount);
    }

    [Member]
    public int Release(Program program) {
      return _semaphore.Release(program);
    }
  }
}

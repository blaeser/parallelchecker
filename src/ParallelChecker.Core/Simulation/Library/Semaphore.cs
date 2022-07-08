using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class Semaphore : Model.Object {
    private int _count;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _unlockTime = new();

    [Member]
    public Semaphore(int initialCount, int maximumCount) : 
      base(typeof(System.Threading.Semaphore)) {
      _count = initialCount;
      if (initialCount < 0 || initialCount > maximumCount) {
        throw new ArgumentException(nameof(initialCount));
      }
    }

    [Member]
    public bool WaitOne(Program program) {
      if (_count == 0) {
        program.Wait(this, _waiting);
      } else {
        program.ActiveThread.NestedLocks++;
        _count--;
        if (_count <= 0) {
          LockHolders.Add(program.ActiveThread);
        }
        program.Pass(_unlockTime);
      }
      return true;
    }

    [Member]
    public int Release(Program program, int releaseCount) {
      var result = _count;
      _count += releaseCount;
      if (_count > 0) {
        ClearLockHolders();
      }
      program.SignalAll(_waiting, _unlockTime);
      return result;
    }

    [Member]
    public int Release(Program program) {
      var result = _count;
      _count++;
      if (_count > 0) {
        ClearLockHolders();
      }
      program.SignalSingle(_waiting, _unlockTime);
      return result;
    }

    private void ClearLockHolders() {
      foreach (var holder in LockHolders) {
        holder.NestedLocks--;
      }
      LockHolders.Clear();
    }
  }
}

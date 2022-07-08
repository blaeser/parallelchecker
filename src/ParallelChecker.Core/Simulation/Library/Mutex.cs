using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class Mutex : Model.Object {
    private bool _locked;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _unlockTime = new();

    [Member]
    public Mutex() :
      this(false) {
    }

    [Member]
    public Mutex(bool locked) :
      base(typeof(System.Threading.Mutex)) {
      _locked = locked;
    }

    [Member]
    public void ReleaseMutex(Program program) {
      ClearLockHolders();
      _locked = false;
      program.SignalSingle(_waiting, _unlockTime);
    }

    [Member]
    public bool WaitOne(Program program) {
      var thread = program.ActiveThread;
      if (_locked) {
        program.Wait(this, _waiting);
      } else {
        LockHolders.Add(thread);
        thread.NestedLocks++;
        program.Pass(_unlockTime);
        _locked = true;
      }
      return true;
    }

    [Member]
    public bool WaitOne(Program program, object _) {
      return WaitOne(program);
    }

    [Member]
    public bool WaitOne(Program program, object _, object _2) {
      return WaitOne(program);
    }

    // TODO: unify with Semaphore
    private void ClearLockHolders() {
      foreach (var holder in LockHolders) {
        holder.NestedLocks--;
      }
      LockHolders.Clear();
    }
  }
}

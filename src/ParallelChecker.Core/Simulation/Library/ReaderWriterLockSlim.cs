using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class ReaderWriterLockSlim : Model.Object {
    // TODO: Support other members and other lock recursion policy
    // TODO: Make fair
    private readonly List<Thread> _readers = new();
    private Thread _writer = null;
    private Thread _upgrader = null;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _unlockTime = new();

    [Member]
    public ReaderWriterLockSlim() 
      : base(typeof(System.Threading.ReaderWriterLockSlim)) {
    }

    [Member]
    public void EnterReadLock(Program program) {
      var thread = program.ActiveThread;
      if (_readers.Contains(thread) || _upgrader == thread || _writer == thread) {
        throw new Model.Exception(program.ActiveLocation, "Lock recursion forbidden");
      }
      if (_writer != null) {
        program.Wait(this, _waiting);
      } else {
        _readers.Add(thread);
        LockHolders.Add(thread);
        program.Pass(_unlockTime);
      }
    }

    [Member]
    public void ExitReadLock(Program program) {
      var thread = program.ActiveThread;
      if (!_readers.Contains(thread)) {
        throw new Model.Exception(program.ActiveLocation, "Read lock not held");
      }
      _readers.Remove(thread);
      program.SignalAll(_waiting, _unlockTime);
      LockHolders.Remove(thread);
    }

    [Member]
    public void EnterUpgradeableReadLock(Program program) {
      var thread = program.ActiveThread;
      if (_readers.Contains(thread) || _upgrader == thread || _writer == thread) {
        throw new Model.Exception(program.ActiveLocation, "Lock recursion forbidden");
      }
      if (_writer != null || _upgrader != null) {
        program.Wait(this, _waiting);
      } else {
        _upgrader = thread;
        LockHolders.Add(thread);
        program.Pass(_unlockTime);
      }
    }

    [Member]
    public void ExitUpgradeableReadLock(Program program) {
      var thread = program.ActiveThread;
      if (_upgrader != thread) {
        throw new Model.Exception(program.ActiveLocation, "Upgradeable read lock not held");
      }
      _upgrader = null;
      program.SignalAll(_waiting, _unlockTime);
      LockHolders.Remove(thread);
    }

    [Member]
    public void EnterWriteLock(Program program) {
      var thread = program.ActiveThread;
      if (_readers.Contains(thread) || _writer == thread) {
        throw new Model.Exception(program.ActiveLocation, "Lock recursion forbidden");
      }
      if (_writer != null || _readers.Count > 0 || _upgrader != thread) {
        if (_upgrader == thread) {
          LockHolders.Remove(thread);
        }
        program.Wait(this, _waiting);
        if (_upgrader == thread) {
          LockHolders.Add(thread);
        }
      } else {
        _writer = thread;
        if (_upgrader == null) {
          LockHolders.Add(thread);
        }
        program.Pass(_unlockTime);
      }
    }

    [Member]
    public void ExitWriteLock(Program program) {
      var thread = program.ActiveThread;
      if (_writer != thread) {
        throw new Model.Exception(program.ActiveLocation, "Writer lock not held");
      }
      _writer = thread;
      program.SignalAll(_waiting, _unlockTime);
      if (_upgrader == null) {
        LockHolders.Remove(thread);
      }
    }
  }
}

using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class ReaderWriterLock : Model.Object {
    // TODO: Support other members
    // TODO: Make fair
    private readonly List<Thread> _readers = new();
    private readonly List<Thread> _writers = new();
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _unlockTime = new();

    [Member]
    public ReaderWriterLock() 
      : base(typeof(System.Threading.ReaderWriterLock)) {
    }

    [Member]
    public void AcquireReaderLock(Program program, object _) {
      var thread = program.ActiveThread;
      if (_writers.Count > 0) {
        program.Wait(this, _waiting);
      } else {
        _readers.Add(thread);
        LockHolders.Add(thread);
        program.Pass(_unlockTime);
      }
    }
    
    [Member]
    public void ReleaseReaderLock(Program program) {
      var thread = program.ActiveThread;
      if (!_readers.Contains(thread)) {
        throw new Model.Exception(program.ActiveLocation, "Read lock not held");
      }
      _readers.Remove(thread);
      program.SignalAll(_waiting, _unlockTime);
      LockHolders.Remove(thread);
    }

    [Member]
    public void AcquireWriterLock(Program program, object _) {
      var thread = program.ActiveThread;
      if ((_writers.Count > 0 && !_writers.Contains(thread)) || _readers.Count > 0) {
        program.Wait(this, _waiting);
      } else {
        _writers.Add(thread);
        LockHolders.Add(thread);
        program.Pass(_unlockTime);
      }
    }

    [Member]
    public void ReleaseWriterLock(Program program) {
      var thread = program.ActiveThread;
      if (!_writers.Contains(thread)) {
        throw new Model.Exception(program.ActiveLocation, "Write lock not held");
      }
      _writers.Remove(thread);
      program.SignalAll(_waiting, _unlockTime);
      LockHolders.Remove(thread);
    }
  }
}

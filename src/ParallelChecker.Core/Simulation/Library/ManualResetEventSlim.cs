using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class ManualResetEventSlim : Model.Object {
    private bool _signaled;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _signalTime = new();

    [Member]
    public ManualResetEventSlim(bool signaled)
      : base(typeof(System.Threading.ManualResetEvent)) {
      _signaled = signaled;
    }

    [Member]
    public ManualResetEventSlim(bool signaled, int _)
     : this(signaled) {
    }

    [Member]
    public ManualResetEventSlim()
      : this(false) {
    }

    [Member]
    public bool GetIsSet() {
      return _signaled;
    }

    [Member]
    public bool Set(Program program) {
      _signaled = true;
      program.SignalAll(_waiting, _signalTime);
      return true;
    }

    [Member]
    public bool Reset() {
      _signaled = false;
      return true;
    }

    [Member]
    public bool Wait(Program program) {
      if (!_signaled) {
        program.Wait(this, _waiting);
      } else {
        program.Pass(_signalTime);
      }
      return true;
    }

    [Member]
    public bool Wait(Program program, object _) {
      return Wait(program);
    }

    [Member]
    public bool Wait(Program program, object _, object _2) {
      return Wait(program);
    }
  }
}

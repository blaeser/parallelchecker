using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class AutoResetEvent : Model.Object {
    private bool _signaled;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _signalTime = new();

    [Member]
    public AutoResetEvent(bool signaled) 
      : base(typeof(System.Threading.AutoResetEvent)) {
      _signaled = signaled;
    }

    [Member]
    public bool Set(Program program) {
      _signaled = true;
      program.SignalSingle(_waiting, _signalTime);
      return true;
    }

    [Member]
    public bool Reset() {
      _signaled = false;
      return true;
    }

    [Member]
    public bool WaitOne(Program program) {
      if (!_signaled) {
        program.Wait(this, _waiting);
      } else {
        program.Pass(_signalTime);
        _signaled = false;
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
  }
}

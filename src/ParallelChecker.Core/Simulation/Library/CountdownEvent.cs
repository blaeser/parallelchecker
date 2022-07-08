using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class CountdownEvent : Model.Object {
    private readonly int _initialCount;
    private int _count;
    private readonly Queue<Thread> _waiting = new();
    private readonly VectorTime _signalTime = new();

    [Member]
    public CountdownEvent(int initialCount) :
      base(typeof(System.Threading.CountdownEvent)) {
      _initialCount = initialCount;
      _count = initialCount;
    }

    [Member]
    public void Reset(int count) {
      _count = count;
    }

    [Member]
    public void Reset() {
      _count = _initialCount;
    }

    [Member]
    public void Wait(Program program) {
      if (_count > 0) {
        program.Wait(this, _waiting);
      } else {
        program.Pass(_signalTime);
      }
    }

    [Member]
    public bool Signal(Program program, int signalCount) {
      if (signalCount < 1) {
        throw new Model.Exception(program.ActiveLocation, "signalCount is less than 1");
      }
      if (signalCount > _count) {
        throw new Model.Exception(program.ActiveLocation, "signalCount is greater than CountDownEvent count");
      }
      _count -= signalCount;
      if (_count == 0) {
        program.SignalAll(_waiting, _signalTime);
        return true;
      }
      return false;
    }

    [Member]
    public bool Signal(Program program) {
      return Signal(program, 1);
    }

    [Member]
    public void AddCount(Program program, int signalCount) {
      if (signalCount < 1) {
        throw new Model.Exception(program.ActiveLocation, "signalCount is less than 1");
      }
      if (_count == 0) {
        throw new Model.Exception(program.ActiveLocation, "CountDownEvent is already set");
      }
      _count += signalCount;
    }

    [Member]
    public void AddCount(Program program) {
      AddCount(program, 1);
    }

    [Member]
    public void Wait(Program program, object _) {
      Wait(program);
    }

    [Member]
    public void Wait(Program program, object _, object _2) {
      Wait(program);
    }
  }
}

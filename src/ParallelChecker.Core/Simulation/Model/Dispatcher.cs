using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Model {
  internal sealed class Dispatcher {
    private readonly Random _random;
    private readonly List<Thread> _waiting = new();
    private Thread _runnable = null;
    private readonly VectorTime _lastTime = new();
    
    public Dispatcher(Random random) {
      _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    public void Register(Thread thread) {
      if (thread == null) {
        throw new ArgumentNullException(nameof(thread));
      }
      if (thread.Dispatcher != null) {
        throw new ArgumentException();
      }
      _waiting.Add(thread);
      thread.Dispatcher = this;
      thread.State = ThreadState.Waiting;
      ScheduleNext();
    }

    public void ScheduleNext() {
      if (_runnable == null) {
        _runnable = SelectNext();
      } else if (_runnable.State == ThreadState.Terminated) {
        var terminated = _runnable;
        _runnable = SelectNext();
        _runnable?.SynchronizeWith(terminated);
      }
      if (_runnable != null) {
        _runnable.Time.SynchronizeWith(_lastTime);
        _runnable.AdvanceTime();
        _lastTime.SynchronizeWith(_runnable.Time);
      }
    }

    private Thread SelectNext() {
      if (_waiting.Count == 0) {
        return null;
      }
      var thread = _waiting[_random.Next(_waiting.Count)];
      Thread earlier;
      while ((earlier = FindEarlier(thread)) != null) {
        thread = earlier;
      }
      _waiting.Remove(thread);
      thread.State = ThreadState.Runnable;
      return thread;
    }

    private Thread FindEarlier(Thread thread) {
      return
        (from other in _waiting
         where other.Time.HappensBefore(thread.Time)
         select other).FirstOrDefault();
    }

    public void PauseCurrent() {
      var current = _runnable;
      current.State = ThreadState.Waiting;
      _runnable = SelectNext();
      if (_runnable != null) {
        current.AdvanceTime();
        _runnable.SynchronizeWith(current);
      }
      _waiting.Add(current);
    }
  }
}

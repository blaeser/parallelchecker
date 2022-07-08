using ParallelChecker.Core.Simulation.Base;
using ParallelChecker.Core.Simulation.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelChecker.Core.Simulation.Library {
  [Type("System.Threading")]
  internal sealed class Barrier : Model.Object {
    private readonly int _participants;
    private int _entered;
    private int _exited;
    private readonly Queue<Thread> _waiting = new();

    [Member]
    public Barrier(int participants) :
      base(typeof(System.Threading.Barrier)) {
      if (participants < 0 || participants > 32767) {
        throw new ArgumentException(nameof(participants));
      }
      _participants = participants;
      _entered = 0;
      _exited = participants;
    }

    [Member]
    public Barrier(int participants, object _) :
      this(participants) {
      // TODO: Support post phase action
    }

    [Member]
    public void SignalAndWait(Program program) {
      var thread = program.ActiveThread;
      if (_entered < _participants) {
        _entered++;
        thread.AdvanceTime();
        if (_entered == _participants) {
          _exited -= _participants;
          _exited++;
          var allThreads = _waiting.Union(new List<Thread>() { thread });
          SynchronizeAll(allThreads);
          thread.AdvanceTime();
          _waiting.NotifyAll();
        } else {
          program.Wait(this, _waiting);
        }
      } else {
        _exited++;
        thread.AdvanceTime();
        if (_exited == _participants) {
          _entered -= _participants;
        }
      }
    }

    private void SynchronizeAll(IEnumerable<Thread> allThreads) {
      var time = new VectorTime();
      foreach (var thread in allThreads) {
        time.SynchronizeWith(thread.Time);
      }
      foreach (var thread in allThreads) {
        thread.Time.SynchronizeWith(time);
      }
    }
  }
}

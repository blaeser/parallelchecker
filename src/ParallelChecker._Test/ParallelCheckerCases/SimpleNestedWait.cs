using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Gate {
    private bool locked = true;

    public void Enter() {
      lock (this) {
        while (locked) {
          NestedWait();
        }
      }
    }

    private void NestedWait() {
      lock (this) {
        Monitor.Wait(this);
      }
    }

    public void Open() {
      lock (this) {
        locked = false;
        Monitor.PulseAll(this);
      }
    }
  }

  class SimpleMonitor {
    static void Main() {
      var race = 0;
      var gate = new Gate();
      new Thread(() => {
        gate.Enter();
        race = 1;
      }).Start();
      Console.Write(race); // no race
      gate.Open();
      Console.Write(race); // race
    }
  }
}

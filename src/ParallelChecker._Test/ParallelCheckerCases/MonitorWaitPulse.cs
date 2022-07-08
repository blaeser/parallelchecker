using System;
using System.Threading;

namespace ParallelChecker._Test {
  class MonitorWaitPulse {
    class BoundedBuffer {
      private int? content;

      public void Put(int item) {
        lock (this) {
          while (content != null) {
            Monitor.Wait(this);
          }
          content = item;
          Monitor.PulseAll(this);
        }
      }

      public int Get() {
        lock (this) {
          while (content == null) {
            Monitor.Wait(this);
          }
          var result = content;
          content = null;
          Monitor.PulseAll(this);
          return (int)result;
        }
      }
    }

    static void Main() {
      const int NofThreads = 2;
      const int RoundsPerThread = 10;
      var buffer = new BoundedBuffer();
      Thread[] producers = new Thread[NofThreads];
      Thread[] consumers = new Thread[NofThreads];
      for (int i = 0; i < NofThreads; i++) {
        producers[i] = new Thread(() =>
        {
          for (int k = 0; k < RoundsPerThread; k++) {
            buffer.Put(k);
          }
        });
        consumers[i] = new Thread(() =>
        {
          for (int k = 0; k < RoundsPerThread; k++) {
            buffer.Get();
          }
        });
      }
      foreach (var thread in producers) {
        thread.Start();
      }
      foreach (var thread in consumers) {
        thread.Start();
      }
      foreach (var thread in producers) {
        thread.Join();
      }
      foreach (var thread in consumers) {
        thread.Join();
      }
      var race = 0;
      new Thread(() => race = 0).Start();
      Console.Write(race);
    }
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  class SimpleSemaphore {
    public static void Main() {
      var semaphore = new Semaphore(0, 2);
      var race = 0;
      new Thread(() =>
      {
        race = 1;
        semaphore.WaitOne();
        race = 1;
      }).Start();
      Console.Write(race);
      semaphore.Release();
    }
  }
}

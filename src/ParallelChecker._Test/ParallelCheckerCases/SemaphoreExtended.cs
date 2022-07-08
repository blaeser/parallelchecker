using System;
using System.Threading;

namespace ParallelChecker._Test {
  class SemaphoreExtended {
    public static void Main() {
      var semaphore = new Semaphore(0, 2);
      var race = 0;
      new Thread(() =>
      {
        semaphore.WaitOne();
        race = 1;
      }).Start();
      new Thread(() =>
      {
        semaphore.WaitOne();
        race = 1;
      }).Start();
      Console.Write(race);
      semaphore.Release(2);
    }
  }
}

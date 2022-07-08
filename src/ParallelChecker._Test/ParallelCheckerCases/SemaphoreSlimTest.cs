using System;
using System.Threading;

namespace ParallelChecker._Test {
  class SemaphoreSlimTest {
    public static void Main() {
      var semaphore = new SemaphoreSlim(0);
      var race = 0;
      new Thread(() =>
      {
        race = 1;
        semaphore.Wait();
        race = 1;
      }).Start();
      Console.Write(race);
      semaphore.Release(2);
    }
  }
}

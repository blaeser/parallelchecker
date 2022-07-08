using System;
using System.Threading;

namespace ParallelChecker._Test {
  class BarrierTest {
    public static void Main() {
      const int N = 10;
      var barrier = new Barrier(N);
      var race = 0;
      for (int index = 0; index < N; index++) {
        new Thread(() =>
        {
          race = 1;
          barrier.SignalAndWait();
          Console.WriteLine(race);
        }).Start();
      }
    }
  }
}

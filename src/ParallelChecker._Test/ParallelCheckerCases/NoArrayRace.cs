using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class NoArrayRace {
    public static void Main() {
      var x = new int[1, 2];
      new Thread(() => x[0, 0] = 0).Start();
      new Thread(() => x[0, 1] = 0).Start();
    }
  }
}

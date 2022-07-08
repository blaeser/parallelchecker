using System;
using System.Threading;

namespace ParallelChecker._Test {
  class PotentialDataRace {
    public static void Main() {
      var x = new Random().Next();
      if (x == 1) {
        new Thread(() => x = 2).Start();
      }
      Console.Write(x);
    }
  }
}

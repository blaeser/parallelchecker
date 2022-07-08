using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class StaticInitRace {
    static int x = 0;

    static StaticInitRace() {
      x++;
    }

    static void Main() {
      Console.Write(x);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

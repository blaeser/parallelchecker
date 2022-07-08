using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class StaticInitRace {
    static int x = 0;

    static StaticInitRace() {
      Task.Run(() => x = 2);
      Console.Write(x);
    }

    static void Main() {
      Console.Write(x);
      Task.Run(() => Console.Write(Other.y));
      Other.Test();
    }
  }
}

class Other {
  public static int y = 1;

  public static void Test() {
    y++;
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  class BooleanOperators {
    static int race;

    public static void Main() {
      var x = false;
      var y = true;
      var and = x && Error(); // conditional
      var or = y || Error(); // conditional
      Console.WriteLine(!x);
      Console.WriteLine(x & y);
      Console.WriteLine(x | y);
      Console.WriteLine(x ^ y);
      Console.WriteLine(race);
    }

    private static bool Error() {
      new Thread(() => race = 1).Start();
      return true;
    }
  }
}

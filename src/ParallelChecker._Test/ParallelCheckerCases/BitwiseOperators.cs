using System;
using System.Threading;

namespace ParallelChecker._Test {
  class BitwiseOperators {
    public static void Main() {
      int x = 0x1234abcd;
      int y = 0x00ff00ff;
      Console.WriteLine(x & y);
      Console.WriteLine(x | y);
      Console.WriteLine(x ^ y);
      Console.WriteLine(~x);
      Console.WriteLine(x << 1);
      Console.WriteLine(x >> 1);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

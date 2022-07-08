using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class NestedMultiDim {
    public static void Main() {
      var x = new int[1, 2][];
      x[0, 1] = new int[3];
      new Thread(() => x[0, 1][0] = 1).Start();
      new Thread(() => x[0, 1][1] = 2).Start();
      new Thread(() => x[0, 1][2] = 3).Start();
    }
  }
}

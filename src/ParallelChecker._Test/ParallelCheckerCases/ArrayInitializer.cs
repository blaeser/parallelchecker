using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class ArrayInitializer {
    public static void Main() {
      int[,] a = { { 1, 2, 3 }, { 4, 5, 6 } };
      int[] b = { 7, 8, 9 };
      int[] c = { };
      var d = new int[] { -1, -2 };
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

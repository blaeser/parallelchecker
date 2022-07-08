using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class MultiDimArray {
    public static void Main(/*string[] args*/) {
      var x = new int[2, 3];
      new Thread(() => x[0, 0] = 0).Start();
      for (int i = 0; i < x.GetLength(0); i++) {
        for (int j = 0; j < x.GetLength(1); j++) {
          x[i, j] = i + j;
          Console.WriteLine(x[i, j]);
        }
      }
    }
  }
}

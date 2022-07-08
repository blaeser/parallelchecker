using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class NestedArray {
    public static void Main(/*string[] args*/) {
      var x = new int[2][];
      for (int i = 0; i < x.Length; i++) {
        x[i] = new int[i];
      }
      for (int i = 0; i < x.Length; i++) {
        for (int j = 0; j < x[i].Length; j++) {
          Console.WriteLine(x[i][j]);
        }
      }
    }
  }
}

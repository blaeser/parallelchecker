using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class SimpleArray {
    public static void Main(/*string[] args*/) {
      int l = 10;
      var x = new int[l];
      x[0] = 1;
      x[1] = x[0];
      for (int i = 0; i < x.Length; i++) {
        x[i]++;
      }
    }
  }
}

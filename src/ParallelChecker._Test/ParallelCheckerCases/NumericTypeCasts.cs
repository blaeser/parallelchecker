using System;

namespace ParallelChecker._Test {
  class NumericTypeCasts {
    public static void Main() {
      long x = 1;
      int y = (int)x;
      float f = 1.0f;
      int z = (int)f;
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

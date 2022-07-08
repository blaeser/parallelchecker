using System;
using System.Threading;

namespace ParallelChecker._Test {
  class OperatorMul {
    public static void Main() {
      byte a = 1;
      short b = 2;
      ushort c = 3;
      int d = 4;
      uint e = 5;
      long f = 6;
      decimal g = 7m;
      var total = a * b * c * d * e * f * g;
      if (total == 5040m) {
        new Thread(() => total = 0).Start();
        Console.WriteLine(total);
      }
      float x = 0.5f;
      double y = 1.5;
      if (x * y * 1 == 0.75) {
        Console.WriteLine(total);
      }
    }
  }
}

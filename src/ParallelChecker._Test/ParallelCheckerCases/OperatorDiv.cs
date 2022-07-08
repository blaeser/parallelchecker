using System;
using System.Threading;

namespace ParallelChecker._Test {
  class OperatorDiv {
    public static void Main() {
      byte a = 255;
      short b = 2;
      ushort c = 2;
      int d = 2;
      uint e = 2;
      long f = 2;
      decimal g = 2;
      var total = a / b / c / d / e / f / g;
      f = 3;
      if (total == 3.5m) {
        new Thread(() => total = 0).Start();
        Console.WriteLine(total);
      }
      float x = 10f;
      double y = 2;
      if (x / y == 5) {
        Console.WriteLine(total);
      }
    }
  }
}

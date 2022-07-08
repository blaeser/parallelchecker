using System;
using System.Threading;

namespace ParallelChecker._Test {
  class OperatorSub {
    public static void Main() {
      byte a = 1;
      short b = 2;
      ushort c = 3;
      int d = 4;
      uint e = 5;
      long f = 6;
      decimal g = 7m;
      var sum = a - b - c - d - e - f - g;
      if (sum == -26m) {
        new Thread(() => sum = 0).Start();
        Console.WriteLine(sum);
      }
      float x = 0.5f;
      double y = 1.5;
      if (x - y - 1 == -2) {
        Console.WriteLine(sum);
      }
    }
  }
}

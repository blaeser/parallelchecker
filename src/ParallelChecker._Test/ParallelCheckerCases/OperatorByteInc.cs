using System;
using System.Threading;

namespace ParallelChecker._Test {
  class OperatorByteInc {
    public static void Main() {
      byte a = 255;
      a += 1;
      if (a == 0) {
        var race1 = 0;
        new Thread(() => race1 = 0).Start();
        Console.WriteLine(race1);
      }
      a -= 1;
      if (a == 255) {
        var race2 = 0;
        new Thread(() => race2 = 0).Start();
        Console.WriteLine(race2);
      }
    }
  }
}


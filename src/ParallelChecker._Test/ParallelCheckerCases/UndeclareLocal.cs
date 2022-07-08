using System;
using System.Threading;

namespace ParallelChecker._Test {
  class UndeclareLocal {
    public static void Main() {
      while (true) {
        var x = 1;
        new Thread(() => Console.Write(x++)).Start();
      }
    }
  }
}

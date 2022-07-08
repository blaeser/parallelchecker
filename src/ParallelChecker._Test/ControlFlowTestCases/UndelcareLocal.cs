using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      while (true) {
        var x = 1;
        new Thread(() => Console.Write(x++)).Start();
      }
    }
  }
}

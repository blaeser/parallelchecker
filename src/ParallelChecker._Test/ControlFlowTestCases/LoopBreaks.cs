using System;

namespace ParallelChecker._Test {
  class LoopBreaks {
    public static void Main() {
    }

    public static void TestWhile() {
      var x = 0;
      while (x >= 0) {
        if (x == 1) {
          break;
        }
        x++;
      }
      Console.Write(x);
    }

    public static void TestDo() {
      var x = 0;
      do {
        if (x == 2) {
          break;
        }
        x++;
      } while (x >= 0);
      Console.Write(x);
    }

    public static void TestFor() {
      for (int i = 0; i >= 0; i++) {
        if (i == 3) {
          break;
        }
        Console.Write(i);
      }
    }
  }
}

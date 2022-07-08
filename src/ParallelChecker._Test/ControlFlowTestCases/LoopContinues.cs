using System;

namespace ParallelChecker._Test {
  class LoopContinues {
    public static void Main() { }

    public static void TestWhile() {
      var x = 0;
      while (x < 10) {
        if (x == 1) {
          continue;
        }
        x++;
      }
    }

    public static void TestDo() {
      var x = 0;
      do {
        if (x == 2) {
          continue;
        }
        x++;
      } while (x < 10);
    }

    public static void TestFor() {
      for (int i = 0; i < 10; i++) {
        if (i == 3) {
          continue;
        }
        Console.Write(i);
      }
    }
  }
}

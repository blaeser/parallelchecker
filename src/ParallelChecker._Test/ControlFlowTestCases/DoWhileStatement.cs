using System;

namespace ParallelChecker._Test {
  class DoWhileStatement {
    public static void Main() {
      int x = 1;
      do {
        x++;
      } while (x < 10);
      Console.WriteLine(x);
    }
  }
}

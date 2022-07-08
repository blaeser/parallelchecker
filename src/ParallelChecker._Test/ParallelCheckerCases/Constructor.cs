using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Constructor {
    public static void Main() {
      var x = new Test(3);
      x.Run();
    }
  }

  public class Test {
    private int x = 1;
    private int y;

    public Test(int y) {
      this.y = y;
      x++;
    }

    public void Run() {
      if (x == 2 && y == 3) {
        new Thread(() => x++).Start();
        Console.Write(x);
      }
    }
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ThisConstructor {
    public static void Main() {
      var x = new Test(3);
      x.Run();
    }
  }

  public class Test {
    private int x = 1;
    private int y;

    public Test(int y) :
      this(5, y) {
      x *= 2;
    }

    public Test(int x, int y) {
      this.y = y;
      this.x = x;
    }

    public void Run() {
      if (x == 10 && y == 3) {
        new Thread(() => x++).Start();
        Console.Write(x);
      }
    }
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  class InfiniteLoop {
    static int x;
    static int y;

    static void Main() {
      Thread t1 = new Thread(() => {
        x = 1;
        while (true) { }
      });
      Thread t2 = new Thread(() => {
        y = 2;
      });
      t1.Start();
      t2.Start();
      t1.Join();
      Console.WriteLine(x);
      t2.Join();
      y = 0;
    }
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  class NestedThreads {
    static int x;
    static int y;

    static void Main() {
      Thread t1 = new Thread(() => {
        x = 1;
        Thread t2 = new Thread(() => {
          y = 2;
        });
        t2.Start();
        t2.Join();
      });
      Thread t3 = new Thread(() => {
        y = 2;
      });
      t1.Start();
      t1.Join();
      t3.Start();
      t3.Join();
      Console.WriteLine(x);
      y = 0;
    }
  }
}

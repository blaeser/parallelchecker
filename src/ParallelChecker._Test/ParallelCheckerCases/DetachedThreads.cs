using System;
using System.Threading;

namespace ParallelChecker._Test {
  class DetachedThreads {
    static int x;
    static int y;

    static void Main() {
      Thread t1 = new Thread(() => {
        x = 1;
        Thread t2 = new Thread(() => {
          y = 2;
        });
        t2.Start();
      });
      Thread t3 = new Thread(() => {
        y = 2;
      });
      t1.Start();
      t3.Start();
      t1.Join();
      t3.Join();
      Console.WriteLine(x);
      y = 0;
    }
  }
}

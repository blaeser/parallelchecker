using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ChildDataRaces {
    static int x;
    static int y;
    static object l = new object();

    static void Main() {
      Thread t1 = new Thread(() => {
        lock (l) {
          x = 1;
        }
      });
      Thread t2 = new Thread(() => {
        lock (l) {
          y = 2;
        }
      });
      t1.Start();
      t2.Start();
      Console.WriteLine(x);
      y = 0;
      t1.Join();
      t2.Join();
    }
  }
}

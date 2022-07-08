using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ParallelDataRace {
    static int x = 1;
    static object l1 = new object();
    static object l2 = new object();

    static void Main() {
      Thread t1 = new Thread(() => {
        lock (l1) {
          x = 1;
        }
      });
      Thread t2 = new Thread(() => {
        lock (l2) {
          Console.WriteLine(x);
        }
      });
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
    }
  }
}

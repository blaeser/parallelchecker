using System;
using System.Threading;

namespace ParallelChecker._Test {
  class DoubleDeadlock {
    static int x = 1;
    static object l1 = new object();
    static object l2 = new object();

    static void Main() {
      Thread t1 = new Thread(() => {
        lock (l1) {
          lock (l2) {
            x = 1;
          }
        }
      });
      Thread t2 = new Thread(() => {
        lock (l2) {
          lock (l1) {
            Console.WriteLine(x);
          }
        }
      });
      t1.Start();
      t2.Start();
      t1.Join();
      t2.Join();
      x = 0;
    }
  }
}

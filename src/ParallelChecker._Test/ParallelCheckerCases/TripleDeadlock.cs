using System;
using System.Threading;

namespace ParallelChecker._Test {
  class TripleDeadlock {
    static int x = 1;
    static object l1 = new object();
    static object l2 = new object();
    static object l3 = new object();

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
          lock (l3) {
            Console.WriteLine(x);
          }
        }
      });
      Thread t3 = new Thread(() => {
        lock (l3) {
          lock (l1) {
            Console.WriteLine(x);
          }
        }
      });
      t1.Start();
      t2.Start();
      t3.Start();
      t1.Join();
      t2.Join();
      t3.Join();
      x = 0;
    }
  }
}

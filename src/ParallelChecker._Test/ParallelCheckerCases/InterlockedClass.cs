using System;
using System.Threading;

namespace ParallelChecker._Test {
  class InterlockedClass {
    static void Main() {
      int x = 0;
      long y = 0;
      double z = 0;
      for (int i = 0; i < 2; i++) {
        new Thread(() =>
        {
          Interlocked.Add(ref x, 1);
          Interlocked.Add(ref y, 1);
          if (Interlocked.CompareExchange(ref x, 1, 0) == 0) {
            Console.Write("Success");
          }
          if (Interlocked.CompareExchange(ref y, 1, 0) == 0) {
            Console.Write("Success");
          }
          if (Interlocked.CompareExchange(ref z, 1, 0) == 0) {
            Console.Write("Success");
          }
          Interlocked.Increment(ref x);
          Interlocked.Increment(ref y);
          Interlocked.Decrement(ref x);
          Interlocked.Decrement(ref y);
          Console.Write(Interlocked.Exchange(ref x, 2));
          Console.Write(Interlocked.Exchange(ref y, 2));
          Console.Write(Interlocked.Exchange(ref z, 2));
          Interlocked.MemoryBarrier();
          Console.Write(Interlocked.Read(ref y));
        }).Start();
      }
    }
  }
}

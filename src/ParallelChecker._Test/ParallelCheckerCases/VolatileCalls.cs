using System;
using System.Threading;

namespace ParallelChecker._Test {
  class VolatileCalls {
    static bool open1 = false;
    static bool open2 = false;

    static void Main() {
      var value = 1;
      new Thread(() => {
        value = 2;
        Volatile.Write(ref open2, true);
        while (!Volatile.Read(ref open1)) { }
      }).Start();
      new Thread(() => {
        Volatile.Write(ref open1, true);
        while (!Volatile.Read(ref open2)) { }
        Console.Write(value);
      }).Start();
    }
  }
}

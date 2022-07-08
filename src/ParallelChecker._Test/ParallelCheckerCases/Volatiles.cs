using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Volatiles {
    static volatile bool open1 = false;
    static volatile bool open2 = false;

    static void Main() {
      var value = 1;
      new Thread(() => {
        value = 2;
        open2 = true;
        while (!open1) { }
      }).Start();
      new Thread(() => {
        open1 = true;
        while (!open2) { }
        Console.Write(value);
      }).Start();
    }
  }
}

using System;
using System.Threading;

namespace QuickSort {
  class ConditionalAnd {
    static void Main(string[] args) {
      if (false && false) {
        var race = 0;
        new Thread(() => race = 1).Start();
        Console.WriteLine(race);
      }
    }
  }
}

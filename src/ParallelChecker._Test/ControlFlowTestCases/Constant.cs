using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Constant {
    const int ConstA = 10;

    static void Main(string[] args) {
      const int ConstB = -20;
      if (ConstA == 10 && ConstB == -20) {
        var race = 0;
        new Thread(() => race = 1).Start();
        Console.Write(race);
      }
    }
  }
}

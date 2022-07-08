using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ForRace {
    static void Main() {
      for (int index = 0; index < 10; index++) {
        Task.Run(() => Console.WriteLine(index));
      }
    }
  }
}

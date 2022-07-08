using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ForeachNoRace {
    static void Main() {
      var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      foreach (var item in array) {
        Task.Run(() => Console.Write(item));
      }
      array = null;
    }
  }
}

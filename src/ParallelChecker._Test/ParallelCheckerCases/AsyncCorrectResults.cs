using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class AsyncCorrectResults {
    private static int race;

    public static void Main() {
      if (Test2().Result == 31) {
        var race1 = 0;
        Task.Run(() => race1 = 1);
        Console.Write(race1);
      }
    }

    private static async Task<int> Test2() {
      if (await Test() == 42) {
        var race2 = 0;
        new Thread(() => race2 = 1).Start();
        Console.Write(race2);
      }
      return 31;
    }

    private static async Task<int> Test() {
      await Task.Run(() => { });
      return 42;
    }
  }
}

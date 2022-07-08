using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {

    public static void Main() {
      try {
        int y = 1 != 2 ? throw new Exception() : 0;
      } catch (Exception) {
        int race = 0;
        Task.Run(() => race = 1);
        Console.Write(race);
      }
    }
  }
}

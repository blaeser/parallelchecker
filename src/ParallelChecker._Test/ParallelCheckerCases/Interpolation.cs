using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Interpolation {
    static void Main(string[] args) {
      int x = 1;
      bool y = true;
      string z = "ABC";
      string a = $"First{x}second{y}{z}";
      if (a == "First1secondTrueABC") {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

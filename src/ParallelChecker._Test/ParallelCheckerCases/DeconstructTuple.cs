using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      (var x, var y) = Test();
      if (x == 1 && y == "Test") {
        int race = 0;
        Task.Run(() => race = 1);
        Console.Write(race);
      } else {
        int noRace = 0;
        Task.Run(() => noRace = 1);
        Console.Write(noRace);
      }
    }

    static (int First, string Second) Test() {
      return (1, "Test");
    }
  }
}

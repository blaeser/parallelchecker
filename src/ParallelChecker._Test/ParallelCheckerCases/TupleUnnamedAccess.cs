using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      var y = Test();
      if (y.Item1 == 1 && y.Item2 == "Test") {
        int race = 0;
        Task.Run(() => race = 1);
        Console.Write(race);
      } else {
        int noRace = 0;
        Task.Run(() => noRace = 1);
        Console.Write(noRace);
      }
    }

    static (int, string) Test() {
      return (1, "Test");
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      var y = Test();
      if (y.First == 1 && y.Second == "Test") {
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

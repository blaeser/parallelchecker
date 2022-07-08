using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      int x = 0;
      switch (x) {
        case 3:
          break;
        case 0:
        case 1:
        case 2:
          var race = 0;
          Task.Run(() => race = 1);
          Console.WriteLine(race);
          break;
        default:
          Console.WriteLine("DEFAULT");
          break;
      }
    }
  }
}

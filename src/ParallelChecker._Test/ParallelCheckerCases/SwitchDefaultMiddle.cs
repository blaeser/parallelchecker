using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      int x = 0;
      switch (x) {
        case 0:
        default:
          Console.WriteLine("CASE 0 AND DEFAULT");
          var race = 0;
          Task.Run(() => race = 1);
          Console.WriteLine(race);
          break;
        case 1:
          Console.WriteLine("CASE 1");
          break;
      }
    }
  }
}

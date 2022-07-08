using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      object x = null;
      switch (x) {
        case 0:
          break;
        case 1:
          break;
        default:
        case null:
          var race = 0;
          Task.Run(() => race = 1);
          Console.WriteLine(race);
          break;
      }
    }
  }
}

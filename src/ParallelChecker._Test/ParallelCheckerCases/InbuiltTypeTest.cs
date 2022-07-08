using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class InbuiltTypeTest {
    static void Main(string[] args) {
      object x = "TEST";
      object y = 11;
      if (x is string && y is int) {
        var race = 0;
        Task.Run(() => race = 1);
        Console.Write(race);
      }
    }
  }
}

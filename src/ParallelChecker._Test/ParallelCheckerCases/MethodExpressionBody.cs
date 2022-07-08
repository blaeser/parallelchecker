using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class MethodExpression {
    static void Main(string[] args) {
      if (Test() == 42) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }

    static int Test() => 42;
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ArithmeticOperators {
    public static void Main() {
      try {
        throw new Exception();
      } catch (Exception) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class TestException : Exception {}

  class ArithmeticOperators {
    public static void Main() {
      try {
        throw new TestException();
      } catch (TestException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test : Exception { }

  class ExceptionTest {

    public static void Main() {
      try {
        try {
          throw new Exception();
        } catch (DivideByZeroException) {

        }
      } catch (Exception) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

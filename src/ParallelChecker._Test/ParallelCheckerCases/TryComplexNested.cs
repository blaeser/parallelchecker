using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test : Exception { }

  class ExceptionTest {

    public static void Main() {
      try {
        _Test();
      } catch (Test) {
        var noRace = 1;
        Task.Run(() => noRace++);
        Console.WriteLine(noRace);
      }
    }

    private static void _Test() {
      try {
        try {
          _Test2();
        } catch (Exception e) {
          throw e;
        }
        return;
      } catch (DivideByZeroException) {

      }
    }

    private static void _Test2() {
      throw new Exception();
    }
  }
}

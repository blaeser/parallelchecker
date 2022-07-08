using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    class Test : System.Exception { }

    class Sub : Test { }

    public static void Main() {
      try {
        throw new Sub();
      } catch (DivideByZeroException) {
        throw new NotImplementedException();
      } finally {
        Console.WriteLine("FINALLY");
      }
      var noRace = 1;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
  }
}

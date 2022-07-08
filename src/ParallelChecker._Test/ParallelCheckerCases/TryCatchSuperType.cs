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
      } catch (Test e) {
        Console.WriteLine("CATCH" + e);
      } finally {
        Console.WriteLine("FINALLY");
      }
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

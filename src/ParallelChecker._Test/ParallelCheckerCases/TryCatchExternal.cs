using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      try {
        var x = 0;
        Console.WriteLine(1 / x);
      } catch (DivideByZeroException) {
        Console.WriteLine("CATCH 1");
      } catch (Exception) {
        throw new NotImplementedException();
      } finally {
        Console.WriteLine("FINALLY");
      }
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

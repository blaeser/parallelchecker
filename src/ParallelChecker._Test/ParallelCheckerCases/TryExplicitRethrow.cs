using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      try {
        try {
          throw new Exception();
        } catch (Exception e) {
          throw e;
        } finally {
          Console.WriteLine("FINALLY");
        }
      } catch (Exception) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

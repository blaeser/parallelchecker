using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      try {
        try {
          throw new Exception();
        } finally {
          Console.WriteLine("FINALLY");
          throw new NullReferenceException();
        }
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

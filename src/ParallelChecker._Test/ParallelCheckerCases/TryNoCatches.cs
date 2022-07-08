using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      try {
        throw new Exception();
      } finally {
        Console.WriteLine("FINALLY");
      }
      var noRace = 1;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
  }
}

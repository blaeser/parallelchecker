using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      try {
        throw new Exception();
      } finally {
        int[] array = null;
        try {
          array[0] = 0;
        } catch (NullReferenceException) {
          var race = 1;
          Task.Run(() => race++);
          Console.WriteLine(race);
        }
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      int[] x = null;
      try {
        Console.Write(x[0]);
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

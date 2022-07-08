using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      int[] x = new int[3];
      try {
        Console.Write(x[3]);
      } catch (IndexOutOfRangeException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

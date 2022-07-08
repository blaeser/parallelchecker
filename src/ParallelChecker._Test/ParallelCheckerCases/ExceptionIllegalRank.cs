using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      int[] x = new int[1];
      try {
        x.GetLength(1);
      } catch (IndexOutOfRangeException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

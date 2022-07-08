using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      try {
        lock (null) {

        }
      } catch (Exception) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

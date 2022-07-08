using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      Semaphore s = null;
      try {
        s.WaitOne();
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

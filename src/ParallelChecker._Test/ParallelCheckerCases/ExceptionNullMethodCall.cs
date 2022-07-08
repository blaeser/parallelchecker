using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test {
    public void F() {
    }
  }

  class ExceptionTest {
    public static void Main() {
      Test x = null;
      try {
        x.F();
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test {
    public int field;
  }

  class ExceptionTest {
    public static void Main() {
      Test x = null;
      try {
        Console.Write(x.field);
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

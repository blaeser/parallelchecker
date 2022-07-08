using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test {
    public int x;

    public int X {
      get {
        return x;
      }
      set {
        x = value;
      }
    }
  }

  class ExceptionTest {
    public static void Main() {
      Test t = null;
      try {
        Console.Write(t.X);
      } catch (NullReferenceException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

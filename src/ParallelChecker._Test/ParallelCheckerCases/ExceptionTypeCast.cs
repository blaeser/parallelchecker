using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A { }
  class B : A { }

  class ExceptionTest {
    public static void Main() {
      A a = new A();
      try {
        B b = (B)a;
      } catch (InvalidCastException) {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test : Exception {
    public string Name { get; }

    public Test(string name) {
      Name = name;
    }
  }

  class ExceptionTest {

    public static void Main() {
      try {
        throw new Test("Hello");
      } catch (Test e) when (e.Name != "Hello") {
        var race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

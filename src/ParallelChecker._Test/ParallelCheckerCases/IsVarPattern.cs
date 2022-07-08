using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A { }

  class Program {
    static void Main(string[] args) {
      Test(new A());
      Test(null);
    }

    private static void Test(A x) {
      while (x is var y) {
        int race = 0;
        Task.Run(() => {
          race = 1;
          y = null;
        });
        Console.Write(race);
      }
    }
  }
}

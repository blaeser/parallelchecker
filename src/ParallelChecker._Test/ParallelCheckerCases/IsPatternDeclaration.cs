using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A { }
  class B : A {
    public int Value => 42;
  }

  class Program {
    static void Main(string[] args) {
      Test(new B());
      Test(new A());
      Test(null);
    }

    private static void Test(A x) {
      while (x is B y && y.Value == 42) {
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

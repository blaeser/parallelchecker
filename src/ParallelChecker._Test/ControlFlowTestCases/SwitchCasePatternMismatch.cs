using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A {
    public int Value => 42;
  }

  class Program {
    static void Main(string[] args) {
      object x = new Program();
      switch (x) {
        case 0:
          break;
        case A y:
          if (y.Value == 42) {
            var race = 0;
            Task.Run(() => race = 1);
            Console.WriteLine(race);
          }
          break;
      }
    }
  }
}

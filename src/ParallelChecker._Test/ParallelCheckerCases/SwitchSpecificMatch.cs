using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A {
    public int Value => 42;
  }

  class AA : A { }

  class Program {
    static void Main(string[] args) {
      object x = new AA();
      switch (x) {
        case AA y:
          if (y.Value == 42) {
            var race = 0;
            Task.Run(() => race = 1);
            Console.WriteLine(race);
          }
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

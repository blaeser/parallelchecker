using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A {
    public int Value => 42;
  }

  class AA : A { }

  class Program {
    static void Main(string[] args) {
      object x = new A();
      switch (x) {
        case AA y:
          break;
        default:
          var race = 0;
          Task.Run(() => race = 1);
          Console.WriteLine(race);
          break;
      }
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A {
    public int Value => 42;
  }

  class Program {
    static void Main(string[] args) {
      object x = new A();
      switch (x) {
        case A y when y.Value < 0:
          Console.Write("FIRST");
          break;
        case A y when y.Value == 42:
          var race = 0;
          Task.Run(() => race = 1);
          Console.WriteLine(race);
          break;
      }
    }
  }
}

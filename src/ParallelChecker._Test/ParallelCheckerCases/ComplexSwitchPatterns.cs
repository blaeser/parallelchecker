using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class A { }

  class B : A {
    public int Value => 42;
  }

  class BB : B { }

  class C : A { }

  class Program {
    static void Main(string[] args) {
      Test(new A());
      Test(new B());
      Test(new C());
      Test(1);
      Test(null);
      Test(2);
    }

    private static void Test(object x) {
      switch (x) {
        case 1:
          Console.Write(1);
          break;
        case BB bb:
          Console.Write(bb.Value);
          goto case 1;
        case B b when b.Value == 42:
          int race = 0;
          Task.Run(() => race = 1);
          Console.Write(race);
          goto default;
        case C c:
          Console.Write("C");
          break;
        default:
          Console.Write("None");
          break;
        case null:
          Console.Write("Null");
          break;
      }
    }
  }
}

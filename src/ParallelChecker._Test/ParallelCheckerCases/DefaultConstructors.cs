using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Base {
    public int x = 123;
  }

  class Sub : Base {
    public int y = 321;
  }

  class SubSub : Sub {
    public SubSub() {
      Console.WriteLine("Test");
    }
  }

  class DefaultConstructors {
    public static void Main() {
      if (new Sub().x == 123 && new SubSub().x == 123 && new Base().x == 123 && new SubSub().y == 321) {
        var race = 0;
        new Thread(() => race = 1).Start();
        Console.WriteLine(race);
      }
    }
  }
}

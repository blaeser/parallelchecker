using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Base {
    protected int x;

    public Base() { }

    public Base(int x) {
      this.x = x;
    }

    public virtual int Get() {
      return 0;
    }

    public virtual int Other() {
      return x * x;
    }
  }

  class Sub : Base {
    private int y;

    public Sub() { }

    public Sub(int x, int y)
      : base(x) {
      this.y = y;
    }

    public override int Get() {
      return base.Get() + x + base.x;
    }
  }

  class Inheritance {
    public static void Main() {
      Base b = new Sub(2, 3);
      if (b.Get() == 2) {
        var first = 0;
        new Thread(() => first = 1).Start();
        Console.WriteLine(first);
      }
      if (b.Other() == 4) {
        var second = 0;
        new Thread(() => second = 1).Start();
        Console.WriteLine(second);
      }
      if (b is Sub) {
        Sub s1 = (Sub)b;
        Sub s2 = b as Sub;
        if (s1 == s2) {
          var third = 0;
          new Thread(() => third = 1).Start();
          Console.WriteLine(third);
        }
      }
    }
  }
}

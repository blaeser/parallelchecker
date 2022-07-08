using System;
using System.Threading;

namespace ParallelChecker._Test {
  interface X {
    void F();
    void H();
  }

  interface Y {
    void G();
    void H();
  }

  class Test : X, Y {
    private int race = 0;

    public void Init() {
      new Thread(() => race = 1).Start();
    }

    public void F() {
      Console.WriteLine("F");
      race++;
    }

    public void G() {
      Console.WriteLine("G");
      race++;
    }

    void X.H() {
      Console.WriteLine("X.H");
      race++;
    }

    void Y.H() {
      Console.WriteLine("Y.H");
      race++;
    }
  }

  class Interfaces {
    public static void Main() {
      X x = new Test();
      ((Test)x).Init();
      x.F();
      x.H();
      Y y = (Y)x;
      y.G();
      y.H();
    }
  }
}

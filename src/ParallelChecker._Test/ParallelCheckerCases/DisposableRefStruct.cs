using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {
  ref struct A {
    public int x, y;

    public void Dispose() {
      int race = 0;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }

  class Program {
    static void Main() {
      using A a = new A {
        x = 1,
        y = 2
      };
      Console.Write(a.x + " " + a.y);
    }
  }
}

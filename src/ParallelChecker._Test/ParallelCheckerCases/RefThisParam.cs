using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int a = 0;
      Task.Run(() => Console.Write(a));
      a.Test1();
      a.Test2();
    }
  }

  static class Extensions {
    public static void Test1(this ref int x) {
      x = 1;
    }

    public static void Test2(ref this int y) {
      y = 2;
    }
  }
}

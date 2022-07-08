using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int a = 0;
      Task.Run(() => a = 1);
      Extensions.Test(a);
      Extensions.Test(in a);
    }
  }

  static class Extensions {
    public static void Test(int x) {
      Console.WriteLine(x);
    }

    public static void Test(in int y) {
      Console.WriteLine(y);
    }
  }
}

using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Test(1);
    }

    private static void Test(double x) {
      Console.WriteLine("OK");
      Task.Run(() => x++);
      Console.WriteLine(x);
    }

    private void Test(int x) {
      throw new NotImplementedException();
    }
  }
}

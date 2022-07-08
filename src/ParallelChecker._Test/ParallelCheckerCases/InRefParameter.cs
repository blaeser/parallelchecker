using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int count = 5;
      Task.Run(() => count++);
      Test(count);
    }

    static void Test(in int x) {
      Console.WriteLine(x);
    }
  }
}

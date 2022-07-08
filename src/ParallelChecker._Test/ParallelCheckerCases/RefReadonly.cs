using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int count = 5;
      Task.Run(() => count++);
      ref readonly int x = ref Test(count);
      Console.WriteLine(x);
    }

    static ref readonly int Test(in int x) {
      Console.WriteLine(x);
      return ref x;
    }
  }
}

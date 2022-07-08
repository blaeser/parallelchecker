using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int x = 0;
      ref int y = ref x;
      Task.Run(() => x++);
      Console.WriteLine(y);
    }
  }
}

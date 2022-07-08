using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int x = default;
      if (x == 0) {
        Task.Run(() => x++);
      }
      Console.Write(x);
    }
  }
}

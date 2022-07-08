using System;
using System.Threading.Tasks;

class X {
  protected private void Test(ref int x) {
    Console.Write(x);
  }
}

namespace CheckerDevTest {
  class Program : X {
    static void Main() {
      int x = 0b_1_00_1;
      if (x == 9) {
        int race = 0;
        Task.Run(() => race++);
        new Program().Test(ref race);
      }
    }
  }
}

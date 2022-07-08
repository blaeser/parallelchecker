using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Test(x: 1, 2, z: 3);
    }

    static void Test(int x, int y, int z) {
      if (x == 1 && y == 2 && z == 3) {
        Task.Run(() => x = y = z = 0);
      }
      Console.WriteLine(x + y + z);
    }
  }
}

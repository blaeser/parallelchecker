using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Test();
    }

    static unsafe void Test() {
      int* x = stackalloc int[3];
      int* y = stackalloc int[] { 1, 2, 3 };
      int* z = stackalloc[] { 1, 2, 3 };
      int* a = stackalloc int[3] { 1, 2, 3 };
      Task.Run(() => x[0]++);
      Console.WriteLine(x[0]);
      Task.Run(() => y[0]++);
      Console.WriteLine(y[0]);
      Task.Run(() => z[0]++);
      Console.WriteLine(z[0]);
      Task.Run(() => a[0]++);
      Console.WriteLine(a[0]);
    }
  }
}

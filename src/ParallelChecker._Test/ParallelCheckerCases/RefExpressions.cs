using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  public class Program {
    public unsafe static void Main() {
      int a = 1, b = 0;
      Task.Run(() => Console.Write(a));
      Test(ref a > b ? ref a : ref b);
    }

    private static void Test(ref int x) {
      x++;
    }
  }
}

using System;
using System.Threading.Tasks;

namespace Empty {
  class Program {
    static void Main(string[] args) {
      for (int i = 0; i < 10; i++) {
        Test(out var a);
        Test(out int b);
        if (a == 42 && b == 42) {
          Task.Run(() => a = 1);
          Task.Run(() => b = 1);
        }
        Console.Write(b);
      }
    }

    static void Test(out int x) {
      x = 42;
    }
  }
}

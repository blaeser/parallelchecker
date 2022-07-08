using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int x = 42;
      Test(ref x);
      if (x == 0) {
        var race = 0;
        Task.Run(() => race++);
        Console.Write(race);
      }
    }
    unsafe static void Test<T>(ref T item) where T : unmanaged {
      fixed (T* p = &item) {
        *p = default;
      }
    }
  }
}

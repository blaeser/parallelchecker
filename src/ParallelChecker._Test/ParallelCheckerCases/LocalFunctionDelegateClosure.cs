using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int outer = 0;
      void Test() {
        Task.Run(() => outer++);
      }
      Call(Test);
      Console.WriteLine(outer);
    }

    static void Call(Action x) {
      x();
    }
  }
}

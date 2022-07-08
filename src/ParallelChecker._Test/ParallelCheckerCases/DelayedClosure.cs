using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int x = 42;

      void Local() {
        if (x == 42) {
          x++;
        }
      }

      Task.Run(() => { }).ContinueWith(t => Local());
      Console.WriteLine(x);
    }
  }
}

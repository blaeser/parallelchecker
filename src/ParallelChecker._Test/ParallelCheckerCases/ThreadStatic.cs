using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1 {
  class Program {
    [ThreadStatic]
    private static int x;

    private static void Main() {
      Task.Run(() => x++);
      Thread.Sleep(100);
      Console.WriteLine(x);
    }
  }
}

using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int x = 0;
      int y = 0;
      var task1 = Task.Run(() => x++);
      var task2 = Task.Run(() => y++);
      var combined = Task.WhenAll(task1, task2);
      combined.Wait();
      Console.WriteLine(x);
      Console.WriteLine(y);
    }
  }
}

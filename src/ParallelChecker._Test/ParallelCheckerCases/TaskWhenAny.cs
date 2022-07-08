using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int x = 0;
      int y = 0;
      var task1 = Task.Run(() => x++);
      var task2 = Task.Run(() => y++);
      var combined = Task.WhenAny(task1, task2);
      if (combined.Result == task1) {
        Console.WriteLine(x);
      } else {
        Console.WriteLine(y);
      }
    }
  }
}

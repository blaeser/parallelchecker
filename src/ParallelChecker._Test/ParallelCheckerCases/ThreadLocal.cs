using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1 {
  class Program {
    private static ThreadLocal<int> x = new ThreadLocal<int>();

    private static void Main() {
      Task.Run(() => {
        x.Value = 1;
        if (x.Value == 1) {
          var race = 0;
          Task.Run(() => race++);
          Console.WriteLine(race);
        }
        x.Value++;
        if (x.Value == 1) {
          var noRace = 0;
          Task.Run(() => noRace++);
          Console.WriteLine(noRace);
        }
      });
      Thread.Sleep(100);
      Console.WriteLine(x);
    }
  }
}

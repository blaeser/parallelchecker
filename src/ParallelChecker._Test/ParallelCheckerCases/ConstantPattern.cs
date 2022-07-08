using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Program x = null;
      if (x is null) {
        var race = 1;
        Task.Run(() => race++);
        Console.Write(race);
      }
    }
  }
}

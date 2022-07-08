using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      var t1 = (1, 2);
      var t2 = (1, 2);
      var t3 = (2, 1);
      if (t1 == t2 && t1 != t3) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

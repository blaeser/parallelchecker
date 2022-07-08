using System;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int[] array = { 1, 2, 3, 4 };
      var query = from x in array where x == 4 select x;
      if (query.Single() == 4) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      if (query.SingleOrDefault() == 4) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      var query2 = from x in array where x < 4 select x;
      try {
        if (query2.Single() == 0) {
          var noRace = 0;
          Task.Run(() => noRace++);
          Console.WriteLine(noRace);
        }
      } catch (InvalidOperationException) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

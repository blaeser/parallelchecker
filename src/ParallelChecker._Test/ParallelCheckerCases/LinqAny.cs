using System;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int[] array = { 1, 2, 3, 4 };
      var query = from x in array where x % 2 == 0 select x;
      if (query.Any()) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      var query2 = from x in array where x > 4 select x;
      if (query2.Any()) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

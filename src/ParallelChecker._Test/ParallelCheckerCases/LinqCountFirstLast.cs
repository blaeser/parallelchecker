using System;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int[] array = { 1, 2, 3, 4 };
      var query = from x in array select x;
      if (query.First() == 1 && query.Last() == 4 && query.Count() == 4 && query.FirstOrDefault() == 1 && query.LastOrDefault() == 4 && query.LongCount() == 4) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

using System;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int[] array1 = { 1, 2 };
      int[] array2 = { 3, 4 };
      int[] array3 = { 2, 3, 4 };
      int[] array4 = { 1, 4 };
      var query = array1.Union(array2).Intersect(array3).Except(array4).Reverse();
      if (query.First() == 3 && query.Count() == 2) {
        var race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      if (query.First() == 2) {
        var noRace = 0;
        Task.Run(() => noRace++);
        Console.WriteLine(noRace);
      }
    }
  }
}

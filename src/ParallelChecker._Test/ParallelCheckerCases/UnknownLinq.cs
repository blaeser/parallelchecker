using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int[] array1 = { 1, 2, 3, 4, 5 };
      int[] array2 = { 6, 7, 8 };
      List<int[]> list = new List<int[]>() {
        array1, array2
      };
      var query =
        from x in list
        from y in x
        select x[0] + y;
      var query2 =
        from x in query
        group x by x;
      var query3 =
        from x in list join y in list on x equals y
        select new { x, y };
      // into

      //var query =
      //  list.SelectMany(x => x);
      int counter = 0;
      foreach (var value in query2) {
        Task.Run(() => counter++);
      }
      foreach (var value in query3) {
        Task.Run(() => counter++);
      }
    }
  }
}

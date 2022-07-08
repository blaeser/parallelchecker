using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      List<int> list = new List<int>() { 1, 2, 3, 4 };
      list.Add(5);
      int side = 0;
      var query = list.AsParallel().AsSequential().Where(x => {
        side++;
        return x > 2;
      }).Where(IsLess);
      int race = 0;
      foreach (var value in query) {
        Task.Run(() => race = value);
      }
    }

    private static bool IsLess(int x) {
      return x < 5;
    }
  }
}

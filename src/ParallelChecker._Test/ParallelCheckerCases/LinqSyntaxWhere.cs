using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      List<int> list = new List<int>() { 1, 2, 3, 4 };
      list.Add(5);
      var query = from x in list where x > 3 select x;
      int counter = 0;
      foreach (var value in query) {
        Task.Run(() => counter += value);
      }
    }
  }
}

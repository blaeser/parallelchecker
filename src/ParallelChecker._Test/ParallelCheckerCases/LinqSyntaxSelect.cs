using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int[] array = { 1, 2, 3, 4, 5 };
      var query = from x in array where x > 3 select x + 3;
      int counter = 0;
      foreach (var value in query) {
        if (value >= 7) {
          Task.Run(() => counter = value);
        }
      }
    }
  }
}

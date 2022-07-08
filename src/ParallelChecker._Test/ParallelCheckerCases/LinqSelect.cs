using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int[] array = { 1, 2, 3, 4, 5 };
      var query = array.Where(x => x > 3).Select(x => x + 1).Select(x => x + 2);
      int counter = 0;
      foreach (var value in query) {
        if (value >= 7) {
          Task.Run(() => counter = value);
        }
      }
    }
  }
}

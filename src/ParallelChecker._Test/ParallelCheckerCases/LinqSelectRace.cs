using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static int global;

    static int Mapping(int x) {
      global++;
      return x + 3;
    }

    static void Main(string[] args) {
      int[] array = { 1, 2, 3, 4, 5 };
      var query = from x in array.AsParallel() where x > 3 select Mapping(x);
      int counter = 0;
      foreach (var value in query) {
        if (value >= 7) {
          Task.Run(() => counter = value);
        }
      }
    }
  }
}

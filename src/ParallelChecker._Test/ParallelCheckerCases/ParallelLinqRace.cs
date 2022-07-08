using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int[] array = { 1, 2, 3, 4, 5 };
      int side = 0;
      var query = array.AsParallel().Where(x => {
        side++;
        return x > 2;
      }).Where(x => x < 5);
      int race = 0;
      foreach (var value in query) {
        Task.Run(() => race = value);
      }
    }
  }
}

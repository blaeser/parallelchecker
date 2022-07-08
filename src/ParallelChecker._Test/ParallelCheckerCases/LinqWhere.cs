using System.Linq;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main(string[] args) {
      int[] array = { 1, 2, 3, 4, 5 };
      var query1 = array.Where(x => x > 3);
      int race = 0;
      foreach (var value in query1) {
        Task.Run(() => race = value);
      }
      var query2 = query1.Where(x => x < 5);
      int noRace = 0;
      foreach (var value in query2) {
        Task.Run(() => noRace = value);
      }
    }
  }
}

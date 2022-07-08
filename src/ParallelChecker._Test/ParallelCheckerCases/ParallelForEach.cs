using System.Collections.Generic;
using System.Threading.Tasks;

class ParallelForEachTest {
  public static void Main() {
    int race = 0;
    List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
    Parallel.ForEach(list, x =>
    {
      race += x;
      x++;
    });
    race++;
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ParallelInvoke {
    static void Main() {
      var race = 0;
      var noRace = 0;
      Parallel.Invoke(() => { race = 1; noRace = 1; }, () => Console.Write(race), () => race++);
      Console.Write(noRace);
    }
  }
}

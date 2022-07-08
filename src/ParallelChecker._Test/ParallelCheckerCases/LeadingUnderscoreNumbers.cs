using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int x = 0b_1_00_1;
      if (x == 9) {
        int race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

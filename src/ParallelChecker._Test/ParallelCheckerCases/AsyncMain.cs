using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    async static Task<int> Main() {
      var race = 0;
      var t = Task.Run(() => race++);
      Console.Write(race);
      await t;
      Console.Write(race);
      return race;
    }
  }
}

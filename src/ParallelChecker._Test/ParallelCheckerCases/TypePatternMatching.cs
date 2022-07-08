using System;
using System.Threading.Tasks;

namespace Test {
  class Program {
    public static void Main() {

      object x = 3;
      if (x is not string) {
        int race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      if (x is not string and not bool) {
        int race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      if (x is string or not bool) {
        int race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
    }
  }
}

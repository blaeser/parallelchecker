using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static int race;
    public static void Main() {
      int x = 0;
      ref int y = ref x;
      y = 2;
      Console.Write(x);
      if (x == 2) {
        Task.Run(() => race = x);
      }
      x = 3;
      Console.Write(y);
      if (y == 3) {
        Task.Run(() => race = x);
      }
      y = 4;
    }
  }
}

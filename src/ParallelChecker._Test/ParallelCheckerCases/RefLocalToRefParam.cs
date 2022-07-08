using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static int race;
    public static void Main() {
      int[] array = { 0, 1 };
      ref int y = ref array[1];
      y = 2;
      Console.Write(array[1]);
      if (array[1] == 2) {
        Task.Run(() => race = array[1]);
      }
      array[1] = 3;
      Console.Write(y);
      if (y == 3) {
        Task.Run(() => race = array[1]);
      }
      y = 4;
    }
  }
}

using System;

namespace ParallelChecker._Test {
  class ForeachArray {
    static void Main() {
      var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      foreach (var item in array) {
        if (item == 8) {
          break;
        } else if (item == 9) {
          return;
        }
        Console.Write(item);
      }
    }
  }
}

using System;

namespace ParallelChecker._Test {
  class ForeachArray {
    static void Main() {
      var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      foreach (var item in array) {
        Console.Write(item);
      }
      foreach (var outer in array) {
        foreach (var inner in array) {
          Console.Write(outer + " " + inner);
        }
        if (outer == 8) {
          break;
        } else if (outer == 9) {
          return;
        }
      }
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

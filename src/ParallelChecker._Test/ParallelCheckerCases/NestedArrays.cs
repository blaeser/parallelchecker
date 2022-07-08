using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class NestedArrays {
    public static void Main(string[] args) {
      var x = new int[10][];
      for (int i = 0; i < x.Length; i++) {
        x[i] = new int[i];
      }
      var t1 = new Thread(() => {
        x[1][0] = 0;
      });
      var t2 = new Thread(() => {
        for (int i = 0; i < x.Length; i++) {
          for (int j = 0; j < x[i].Length; j++) {
            Console.Write(x[i][j]);
          }
        }
      });
      t1.Start();
      t2.Start();
    }
  }
}

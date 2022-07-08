using System;
using System.Threading;

namespace ParallelChecker._Test {
  public class Arrays {
    public static void Main(string[] args) {
      var x = new int[10];
      var t1 = new Thread(() => {
        for (int i = 0; i < x.Length; i++) {
          Console.Write(x[i]);
        }
      });
      var t2 = new Thread(() => {
        for (int i = 0; i < x.Length; i++) {
          x[i] = i;
        }
      });
      t1.Start();
      t2.Start();
      x[0]++;
      t1.Join();
      t2.Join();
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class TaskStartJoins {
    static int x = 1;

    static void Main() {
      var t1 = Task.Run(() => {
        x = 1;
      });
      var t2 = Task.Run(() => {
        Console.WriteLine(x);
      });
      t1.Wait();
      t2.Wait();
      x = 2;
    }
  }
}

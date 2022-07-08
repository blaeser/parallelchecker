using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class TaskStartJoins {
    static int x = 1;

    static void Main() {
      var t1 = new Task(() => {
        x = 1;
      });
      var t2 = new Task(() => {
        Console.WriteLine(x);
      });
      t1.Start();
      t2.Start();
      t1.Wait();
      t2.Wait();
    }
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class OpenParameterList {
    public static void Main() {
      var task1 = Task.Run(() => 1);
      var task2 = Task.Run(() => 2);
      Task.WaitAll(new[] { task1, task2 });
    }
  }
}

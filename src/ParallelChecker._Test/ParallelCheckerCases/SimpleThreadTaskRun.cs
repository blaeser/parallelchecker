using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class SimpleThreadTaskRun {
    public static void Main() {
      var x = 0;
      new Thread(() => x = 0).Start();
      Task.Run(() => x = 1);
    }
  }
}

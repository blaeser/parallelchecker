using System;
using System.Threading;

namespace ParallelChecker._Test {
  class BeforeStart {
    static int x;

    static void Main() {
      var t = new Thread(() => {
          x = 1;
      });
      x = 0;
      t.Start();
    }
  }
}

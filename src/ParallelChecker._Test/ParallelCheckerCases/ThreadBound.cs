using System.Threading;

namespace ParallelChecker._Test {
  class ThreadBound {
    static void Main() {
      while (true) {
        new Thread(() => { }).Start();
      }
    }
  }
}

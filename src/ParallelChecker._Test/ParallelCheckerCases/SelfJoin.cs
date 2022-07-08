using System.Threading;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Thread.CurrentThread.Join();
    }
  }
}

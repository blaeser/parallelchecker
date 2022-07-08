using System.Collections.Generic;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      var dict = new List<string>();
      lock (dict) {
        System.Threading.Tasks.Task.Run(() => { });
      }
    }
  }
}

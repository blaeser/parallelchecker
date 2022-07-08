using System.Collections.Generic;

namespace ParallelChecker._Test {
  class Yield {
    public static void Main() {
      var z = Items();
      System.Threading.Tasks.Task.Run(() => { });
    }

    private static IEnumerable<int> Items() {
      for (int i = 0; i < 10; i++) {
        yield return i;
      }
    }
  }
}

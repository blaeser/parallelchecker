using System;

namespace ParallelChecker._Test {
  class Nullable {
    public static void Main() {
      int? x = null;
      var y = x ?? 1;

      string s = null;
      s = s ?? "";
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

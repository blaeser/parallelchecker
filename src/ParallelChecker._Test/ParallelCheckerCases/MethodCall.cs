using System;

namespace ParallelChecker._Test {
  class MethodCall {
    public static void Main() {
      new MethodCall().F();
      System.Threading.Tasks.Task.Run(() => { });
    }

    void F() {
      G();
    }

    void G() {
    }
  }
}

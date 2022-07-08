using System;

namespace ParallelChecker._Test {
  class MethodCall {
    public static void Main() {
      new MethodCall().F();
    }

    void F() {
      G();
    }

    void G() {
    }
  }
}

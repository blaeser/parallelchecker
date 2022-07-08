using System;

namespace ParallelChecker._Test {
  class MethodExpression {
    static void Main(string[] args) {
      if (Test() == 42) {
        throw new Exception();
      }
    }

    static int Test() => 42;
  }
}

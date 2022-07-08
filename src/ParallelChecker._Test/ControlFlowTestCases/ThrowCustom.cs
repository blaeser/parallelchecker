using System;

namespace ParallelChecker._Test {
  class TestException : Exception {}

  class ArithmeticOperators {
    public static void Main() {
      throw new TestException();
    }
  }
}

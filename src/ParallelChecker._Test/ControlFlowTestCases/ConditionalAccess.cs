using System;

namespace ParallelChecker._Test {
  class Test {
    public int field;
  }

  class ConditionalOperators {
    public static void Main() {
      Test t = new Test();
      var f = t?.field;
      t = null;
      Console.WriteLine(t?.field);

      int[] a = new int[1];
      var x = a?[0];
      a = null;
      Console.WriteLine(a?[0]);
    }
  }
}

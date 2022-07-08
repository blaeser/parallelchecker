using System;

namespace ParallelChecker._Test {
  class Gotos {
    public static void Main() {
      int i = 0;
      label1:
      i++;
      if (i > 10) goto label2;
      goto label1;
      label2:
      Console.Write("End");
    }
  }
}

using System;

namespace ParallelChecker._Test {
  class OpenParameterList {
    public static void Main() {
      Test("");
      Test("", 1);
      Test("", 2, 3, 4, 5);
      Test("", new int[] { 1, 2 });
    }

    static void Test(string prefix, params int[] values) {
      for (int index = 0; index < values.Length; index++) {
        Console.Write(values[index]);
      }
    }
  }
}

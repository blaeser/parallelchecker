using System;
using System.Collections.Generic;

namespace ParallelChecker._Test {
  class YieldComplex {
    public static void Main() {
      var z = GetEventRecords();
    }

    private static IEnumerable<string> GetEventRecords() {
      if (new Random().Next() == 1)
        yield return "A";
    }
  }
}

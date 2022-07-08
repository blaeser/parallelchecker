using System;
using System.Collections.Generic;

class Test {
  public static void Main() {
    var set1 = new HashSet<string> { "A", "B", "C" };
    var set2 = new HashSet<string>() { "A", "B", "C" };
    var set3 = new HashSet<string>() { { "A" }, { "B" }, "C" };
    if (set1.Contains("A") && set2.Contains("B") && set3.Contains("C")) {
      throw new Exception("OK");
    }
  }
}

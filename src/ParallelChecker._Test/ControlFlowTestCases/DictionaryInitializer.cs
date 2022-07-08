using System;
using System.Collections.Generic;

class Test {
  public static void Main() {
    var dict = new Dictionary<string, int> { { "A", 1 }, { "B", 2 }, { "C", 3 } };
    if (dict["A"] + dict["B"] + dict["C"] == 6) {
      throw new Exception("OK");
    }
  }
}

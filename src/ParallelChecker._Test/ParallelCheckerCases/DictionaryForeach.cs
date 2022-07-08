using System;
using System.Collections.Generic;

class Test {
  public static void Main() {
    var dict = new Dictionary<string, int> { { "A", 1 }, { "B", 2 }, { "C", 3 } };
    foreach (var item in dict.Keys) {
      Console.Write(item);
    }
    foreach (var item in dict.Values) {
      Console.Write(item);
    }
    foreach (var pair in dict) {
      Console.Write(pair.Key + " " + pair.Value);
    }
    System.Threading.Tasks.Task.Run(() => { });
  }
}

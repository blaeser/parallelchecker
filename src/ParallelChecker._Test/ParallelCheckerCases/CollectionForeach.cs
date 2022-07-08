using System;
using System.Collections.Generic;

class Test {
  public static void Main() {
    var coll = new List<int> { 1, 2, 3, 4, 5, 6, 7 };
    foreach (var item in coll) {
      Console.Write(item);
    }
    System.Threading.Tasks.Task.Run(() => { });
  }
}

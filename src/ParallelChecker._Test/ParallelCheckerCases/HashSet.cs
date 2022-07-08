using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var set = new HashSet<string>();
    set.Add("Hello");
    if (set.Count == 1 && set.Contains("Hello")) {
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

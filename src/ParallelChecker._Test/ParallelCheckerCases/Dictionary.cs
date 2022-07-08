using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var dict = new Dictionary<string, int>();
    dict.Add("First", 1);
    dict.Add("Second", 2);
    if (dict.Count == 2 && dict["First"] == 1 && dict["Second"] == 2) {
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

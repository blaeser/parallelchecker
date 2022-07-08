using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var x = new List<int>();
    x.Add(1);
    x.Add(2);
    x[1] = 3;
    if (x.Contains(1) && x.Count == 2 && x[1] == 3) {
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    } else {
      throw new Exception();
    }
  }
}

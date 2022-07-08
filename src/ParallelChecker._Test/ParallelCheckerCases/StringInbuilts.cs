using System;
using System.Threading.Tasks;

class StringTest {
  public static void Main() {
    var x = string.Empty;
    var y = string.IsNullOrEmpty(x) && string.IsNullOrWhiteSpace(x);
    var z = x.Length;
    if (y && z == 0) {
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

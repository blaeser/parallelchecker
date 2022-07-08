using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var size = -1;
    try {
      var x = new int[size];
    } catch (Exception) {
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

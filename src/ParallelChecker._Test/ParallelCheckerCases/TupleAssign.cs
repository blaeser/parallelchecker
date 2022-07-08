using System;
using System.Threading.Tasks;

class Test {
  class Point {
    public int X { get; set; }
    public int Y { get; set; }
  }

  public static void Main() {
    var point = new Point();
    (point.X, point.Y) = (1, 2);
    var (a, b) = (1, 2);
    (var x, var y) = (1, 2);
    if (point.X == 1 && point.Y == 2 && a == 1 && b == 2 && x == 1 && y == 2) {
      var race = 0;
      Task.Run(() => race++);
      Console.Write(race);
    }
  }
}

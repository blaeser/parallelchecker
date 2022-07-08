using System;
using System.Threading.Tasks;

class Test {
  class Point {
    public int X { get; set; }
    public int Y { get; set; }

    public void Deconstruct(out int x, out int y) =>
        (x, y) = (X, Y);
  }

  public static void Main() {
    var point = new Point();
    (point.X, point.Y) = (1, 2);
    var (a, b) = point;
    (var x, var y) = point;
    if (point.X == 1 && point.Y == 2 && a == 1 && b == 2 && x == 1 && y == 2) {
      var race = 0;
      Task.Run(() => race++);
      Console.Write(race);
    }
  }
}
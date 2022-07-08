using System;
using System.Threading.Tasks;

class Test {

  public static void Main() {
    var point = new Point();
    Task.Run(() => { (var x1, var y1) = point; });
    (var x2, var y2) = point;
  }

  class Point {
    public int Deconstructions { get; private set; }

    public int X { get; set; }
    public int Y { get; set; }

    public void Deconstruct(out int x, out int y) {
      x = X;
      y = Y;
      ++Deconstructions;
    }
  }
}

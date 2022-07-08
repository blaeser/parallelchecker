using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  public ref struct Point {
    public Point(double x, double y, double z) {
      X = x;
      Y = y;
      Z = z;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
  }

  class Program {
    static void Main() {
      var p = new Point(1, 2, 3);
      var p2 = p;
      // p cannot be used in Task.Run lambda => no data races with ref struct (stack-bound)
      Test(p);
      Console.WriteLine(p2.X);
      System.Threading.Tasks.Task.Run(() => { });
    }

    static void Test(Point point) {
      Console.WriteLine(point.X);
    }
  }
}

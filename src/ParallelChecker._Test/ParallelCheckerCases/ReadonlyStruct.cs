using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  readonly public struct ReadonlyPoint3D {
    public ReadonlyPoint3D(double x, double y, double z) {
      X = x;
      Y = y;
      Z = z;
    }

    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    private static readonly ReadonlyPoint3D origin = new ReadonlyPoint3D();
    public static ref readonly ReadonlyPoint3D Origin => ref origin;
  }

  class Program {
    static void Main() {
      var p = new ReadonlyPoint3D(1, 2, 3);
      var p2 = p;
      Task.Run(() => p = new ReadonlyPoint3D(1, 2, 3));
      Test(p);
      Console.WriteLine(p2.X);
    }

    static void Test(ReadonlyPoint3D point) {
      Console.WriteLine(point.X);
    }
  }
}

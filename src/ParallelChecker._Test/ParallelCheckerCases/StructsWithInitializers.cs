using System;
using System.Threading.Tasks;

struct Pair {
  private int x = 0;
  private int y = 0;
  public int Z { get; } = -1;
  public int X => x;
  public int Y => y;

  public Pair(int x, int y) {
    this.x = x;
    this.y = y;
  }

  public Pair(int x, int y, int z) : this(x, y) {
    Z = z;
  }
}

class Test {
  public static void Main() {
    var p1 = new Pair(1, 2);
    if (p1.X == 1 && p1.Y == 2 && p1.Z == -1) {
      Task.Run(() => p1 = new Pair(3, 4));
      Console.WriteLine(p1.X);
    }
    var p2 = new Pair();
    if (p2.X == 0 && p2.Y == 0 && p2.Z == 0) {
      Task.Run(() => p2 = new Pair(3, 4, 5));
      Console.WriteLine(p2.X);
    }
    Pair p3 = default;
    if (p3.X == 0 && p3.Y == 0 && p3.Z == 0) {
      Task.Run(() => p3 = new Pair(3, 4, 5));
      Console.WriteLine(p3.X);
    }
  }
}

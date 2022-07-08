using System;
using System.Threading.Tasks;

struct Pair {
  private int x = 0;
  private int y = 0;
  public int Z { get; } = -1;
  public int X => x;
  public int Y => y;

  public Pair() : this(1, 2, 3) {
  }

  public Pair(int x, int y, int z) {
    this.x = x;
    this.y = y;
    Z = z;
  }
}

class Test {
  public static void Main() {
    var p1 = new Pair();
    if (p1.X == 1 && p1.Y == 2 && p1.Z == 3) {
      Task.Run(() => p1 = new Pair(3, 4, 5));
      Console.WriteLine(p1.X);
    }
    Pair p2 = default;
    if (p2.X == 0 && p2.Y == 0 && p2.Z == 0) {
      Task.Run(() => p2 = new Pair(3, 4, 5));
      Console.WriteLine(p2.X);
    }
  }
}

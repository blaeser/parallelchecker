using System;
using System.Threading.Tasks;

struct Pair {
  private int x = 0;
  private int y = 0;
  public int Z { get; set; } = -1;
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
    var p = new Pair() with { Z = 4 };
    if (p.X == 1 && p.Y == 2 && p.Z == 4) {
      Task.Run(() => p = new Pair(3, 4, 5));
      Console.WriteLine(p.X);
    }
  }
}

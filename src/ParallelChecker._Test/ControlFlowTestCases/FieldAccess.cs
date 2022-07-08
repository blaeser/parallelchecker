using System;

namespace ParallelChecker._Test {
  class Pair {
    public int x = 1, y = 2;
    public static int z = 3;
  }

  public class FieldAccess {
    public static void Main() {
      var p1 = new Pair();
      p1.x = p1.y;
      var p2 = new Pair();
      Console.WriteLine(p2.x);
      Pair.z = 4;
      Console.WriteLine(Pair.z);
    }
  }
}

namespace ParallelChecker._Test {
  class AnyTypeDeconstruction {
    public static void Main() {
      var point = new Point();
      (var x, var y) = point;
    }
  }

  public class Point {
    public int X { get; set; }
    public int Y { get; set; }

    public void Deconstruct(out int x, out int y) {
      x = X;
      y = Y;
    }
  }
}
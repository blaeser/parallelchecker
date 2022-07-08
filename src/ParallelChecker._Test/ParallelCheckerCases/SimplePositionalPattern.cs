using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {
  public class Point {
    public int X { get; }
    public int Y { get; }

    public Point(int x, int y) => (X, Y) = (x, y);

    public void Deconstruct(out int x, out int y) =>
        (x, y) = (X, Y);
  }

  class Program {
    static void Main() {
      var point = new Point(1, 2);
      var result = point switch
      {
        (0, 1) => 1,
        (1, 3) => 3,
        (_, _) => 0
      };
      if (result == 0) {
        Task.Run(() => result++);
      }
      Console.Write(result);
    }
  }
}

global using System;
global using System.Threading.Tasks;

namespace MyProgram;


record Point(int X, int Y) {
}

class Test {
  public static void Main() {
    Point p = new(1, 2);
    int x;
    (x, int y) = p;
    if (x == 1 && y == 2) {
      Task.Run(() => p = new(3, 4));
      Console.WriteLine(p.X);
    }
  }
}

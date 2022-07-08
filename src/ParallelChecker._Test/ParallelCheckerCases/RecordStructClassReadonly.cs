using System;
using System.Threading.Tasks;

record class PairClass(int X, int Y) {
  public int Mutable { get; set; }
}

record struct PairStruct(int X, int Y) {
  public int Mutable { get; set; } = 0;
}

readonly record struct PairReadonly(int X, int Y) {
}

class Test {
  public static void Main() {
    PairClass p0 = new(1, 2);
    Task.Run(() => p0.Mutable++);
    Console.WriteLine(p0.Mutable);

    PairClass p1 = p0;
    Task.Run(() => p0.Mutable++);
    Console.WriteLine(p1.Mutable);

    PairStruct p2 = new(1, 2);
    Task.Run(() => p2.Mutable++);
    Console.WriteLine(p2.Mutable);

    PairStruct p3 = p2;
    Task.Run(() => p2.Mutable++);
    Console.WriteLine(p3.Mutable);

    PairReadonly p4 = new(1, 2);
    Task.Run(() => p4 = new(3, 4));
    Console.WriteLine(p4.X);
  }
}

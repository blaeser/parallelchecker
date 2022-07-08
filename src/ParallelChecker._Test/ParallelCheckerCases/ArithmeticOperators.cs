using System;

namespace ParallelChecker._Test {
  class ArithmeticOperators {
    public static void Main() {
      var x = 123;
      Console.Write(+x);
      Console.Write(-x);
      Console.WriteLine(x++);
      Console.WriteLine(x--);
      Console.WriteLine(++x);
      Console.WriteLine(--x);
      var y = 456;
      Console.WriteLine(x + y);
      Console.WriteLine((x + y));
      Console.WriteLine(x - y);
      Console.WriteLine(x * y);
      Console.WriteLine(x / y);
      Console.WriteLine(x % y);
      Console.WriteLine(x == y);
      Console.WriteLine(x != y);
      Console.WriteLine(x < y);
      Console.WriteLine(x <= y);
      Console.WriteLine(x > y);
      Console.WriteLine(x >= y);
      Console.WriteLine(x = y);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

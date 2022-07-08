using System;

namespace ParallelChecker._Test {
  class Test {
    public int X { get; set; } = 1;

    private int _y;

    public int Y {
      get { return _y; }
      set { _y = value; }
    }

    public int Z { get; } = 2;

    public int Fix {
      get { return 3; }
    }

    public static int Static { get; set; } = 4;

    private static int static2;

    public static int Static2 {
      get { return static2; }
      set { static2 = value; }
    }
  }

  public class FieldAccess {
    public static void Main() {
      var a = new Test();
      a.X++;
      Console.WriteLine(a.X);
      a.Y = 5;
      Console.WriteLine(a.Y);
      Console.WriteLine(a.Z);
      Console.WriteLine(a.Fix);
      Test.Static = 6;
      Console.WriteLine(Test.Static);
      Test.Static2 = 7;
      Console.WriteLine(Test.Static2);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

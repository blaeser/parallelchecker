using System;

namespace ParallelChecker._Test {
  class Test {
    private void Display(int height = 10, int width = 10) {
      Console.WriteLine(height);
      Console.WriteLine(width);
    }

    public static void Main() {
      var x = new Test();
      x.Display();
      x.Display(5);
      x.Display(5, 20);
      x.Display(height: 5);
      x.Display(width: 20);
      x.Display(width: 20, height: 5);
    }
  }
}

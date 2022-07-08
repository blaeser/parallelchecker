using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Delegates {
    private delegate void MyDelegate1(int value);
    private delegate bool MyDelegate2(int x, int y);

    public static void Main() {
      MyDelegate1 del1 = Run;
      del1(3);
      var instance = new Delegates();
      MyDelegate2 del2 = instance.Evaluate;
      Console.WriteLine(del2(2, 3));
    }

    private static void Run(int value) {
      ThreadStart start = () => value = 1;
      new Thread(start).Start();
      Console.Write(value);
    }

    private int field = 0;

    private bool Evaluate(int x, int y) {
      new Thread(Set).Start();
      return field > 0;
    }

    private void Set() {
      field = 1;
    }
  }
}

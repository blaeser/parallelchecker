using System;
using System.Threading;

class Test {
  public static void Main() {
    var s = new ThreadStart(Logic);
    new Thread(s).Start();
  }

  private static void Logic() {
    Console.Write("Test");
  }
}
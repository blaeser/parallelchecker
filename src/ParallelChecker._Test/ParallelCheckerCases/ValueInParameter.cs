using System;
using System.Threading.Tasks;

class Test {
  private static void Do(in int x) {
    Console.WriteLine(x);
  }

  public static void Main() {
    Do(1);
    Do(1 + 2);
    Task.Run(() => { });
  }
}

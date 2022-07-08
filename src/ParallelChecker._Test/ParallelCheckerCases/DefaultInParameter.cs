using System;
using System.Threading.Tasks;

class Test {
  private static void Do(in int x) {
    Console.WriteLine(x);
  }

  public static void Main() {
    Do(default(int));
    Do(default);
    Task.Run(() => { });
  }
}

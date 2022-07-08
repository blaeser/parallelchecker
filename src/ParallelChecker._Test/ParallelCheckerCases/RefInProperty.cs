using System;
using System.Threading.Tasks;

class Test {
  private int value;
  private ref int Value => ref value;

  private static void Do(in int x) {
    Console.WriteLine(x);
  }

  private void Sample() {
    Do(Value);
  }

  public static void Main() {
    new Test().Sample();
    Task.Run(() => { });
  }
}

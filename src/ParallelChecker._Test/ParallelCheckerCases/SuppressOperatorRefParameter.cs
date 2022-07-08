using System;
using System.Threading.Tasks;

class Test {
  private static void Do(ref string value) {
    Console.WriteLine(value);
  }

  public static void Main() {
    string? x = "Test";
    Task.Run(() => x = null);
    Do(ref x!);
  }
}

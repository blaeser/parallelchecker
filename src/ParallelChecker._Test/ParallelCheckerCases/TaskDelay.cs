using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    TestAsync().Wait();
  }

  private static async Task TestAsync() {
    await Task.Delay(100);
    int race = 0;
    _ = Task.Run(() => race++);
    Console.WriteLine(race);
  }
}

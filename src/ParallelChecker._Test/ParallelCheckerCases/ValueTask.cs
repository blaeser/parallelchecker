using System.Threading.Tasks;

class Test {
  private static int race, noRace;

  public static async Task Main() {
    var task1 = TestAsync();
    var task2 = Test2Async();
    await task1;
    await task2;
    await Test3Async();
    await Test4Async();
  }

  private static async ValueTask TestAsync() {
    await Task.Run(() => race++);
  }

  private static async ValueTask<int> Test2Async() {
    await Task.Delay(10);
    return race;
  }

  private static async ValueTask Test3Async() {
    await Task.Run(() => noRace++);
  }

  private static async ValueTask<int> Test4Async() {
    await Task.Delay(10);
    return noRace;
  }
}

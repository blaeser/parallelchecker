using System;
using System.Threading.Tasks;

class ContinuationTest {
  public static void Main() {
    var race = 0;
    var first = Task.Run(() =>
    {
      return 11;
    });
    var second = Task.Run(() =>
    {
      return 22;
    });
    first.Wait();
    second.Wait();
    Task.Factory.ContinueWhenAll(new Task<int>[] { first, second }, previous =>
    {
      if (previous[0].Result == 11 && previous[1].Result == 22) {
        Console.Write(race);
      }
    });
    race = 1;
  }
}

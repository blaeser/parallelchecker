using System;
using System.Threading;
using System.Threading.Tasks;

class ContinuationTest {
  public static void Main() {
    var sem = new SemaphoreSlim(0);
    var race = 0;
    Task.Run(() =>
    {
      sem.Wait();
      return 42;
    }).ContinueWith(preceding =>
    {
      if (preceding.Result == 42) {
        Console.Write(race);
      }
    });
    race = 1;
    sem.Release();
    race = 2;
  }
}

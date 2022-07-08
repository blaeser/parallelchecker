using System;
using System.Threading;
using System.Threading.Tasks;

class ManualResetEventSlimTest {
  public static void Main() {
    int race = 0;
    var ev = new ManualResetEventSlim();
    Task.Run(() =>
    {
      ev.Wait();
      race = 0;
    });
    race = 1;
    ev.Set();
    race = 2;
    ev.Reset();
    Task.Run(() =>
    {
      ev.Wait();
      race = 3;
    });
    Console.Write(race);
  }
}

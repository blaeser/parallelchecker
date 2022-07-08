using System;
using System.Threading;
using System.Threading.Tasks;

class ManualResetEventTest {
  public static void Main() {
    int race = 0;
    var ev = new ManualResetEvent(false);
    Task.Run(() =>
    {
      ev.WaitOne();
      race = 0;
    });
    race = 1;
    ev.Set();
    race = 2;
    ev.Reset();
    Task.Run(() =>
    {
      ev.WaitOne();
      race = 3;
    });
    Console.Write(race);
  }
}

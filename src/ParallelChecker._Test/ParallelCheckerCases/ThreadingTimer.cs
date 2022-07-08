using System;
using System.Threading;

class TimerTest {
  public static void Main() {
    int race = 0;
    var timer = new Timer(state =>
    {
      Console.Write("TICK");
      race = 1;
    }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    Console.Write(race);
    timer.Dispose();
    Console.Write(race);
    Console.ReadLine();
  }
}

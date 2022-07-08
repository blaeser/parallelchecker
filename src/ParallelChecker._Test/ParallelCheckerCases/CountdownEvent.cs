using System;
using System.Threading;
using System.Threading.Tasks;

class CountdownEventTest {
  public static void Main() {
    const int N = 10;
    int race = 0;
    var latch = new CountdownEvent(N);
    Task.Run(() =>
    {
      latch.Wait();
      race = 1;
    });
    for (int i = 0; i < N; i++) {
      Console.Write(race);
      latch.Signal();
    }
    Console.Write(race);
  }
}

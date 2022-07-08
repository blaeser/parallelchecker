using System;
using System.Threading;
using System.Threading.Tasks;

class MutexTest {
  public static void Main() {
    int race = 0;
    var mutex = new Mutex();
    Task.Run(() => {
      mutex.WaitOne();
      race = 1;
      mutex.ReleaseMutex();
    });
    mutex.WaitOne();
    Console.Write(race);
    mutex.ReleaseMutex();
    race = 2;
  }
}

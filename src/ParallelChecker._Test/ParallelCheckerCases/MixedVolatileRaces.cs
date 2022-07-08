using System.Threading;

namespace ParallelChecker._Test {
  class MixedVolatileRaces {
    static bool open1 = false;
    static bool open2 = false;

    static void Main() {
      new Thread(() => {
        open2 = true;
        while (!open1) { }
      }).Start();
      new Thread(() => {
        Volatile.Write(ref open1, true);
        while (!Volatile.Read(ref open2)) { }
      }).Start();
    }
  }
}

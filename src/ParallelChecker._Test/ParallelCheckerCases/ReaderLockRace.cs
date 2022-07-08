using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ReaderLockRace {
    public static void Main() {
      const int N = 10;
      var rwLock = new ReaderWriterLock();
      var race = 0;
      for (int index = 0; index < N; index++) {
        new Thread(() =>
        {
          rwLock.AcquireReaderLock(int.MaxValue);
          Console.Write(race++);
          rwLock.ReleaseReaderLock();
        }).Start();
      }
    }
  }
}

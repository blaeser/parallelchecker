using System;
using System.Threading;

namespace ParallelChecker._Test {
  class SimpleAfterLock {
    public static void Main() {
      const int N = 2;
      var rwLock = new ReaderWriterLock();
      var race = 0;
      for (int index = 0; index < N; index++) {
        new Thread(() =>
        {
          rwLock.AcquireWriterLock(int.MaxValue);
          Console.Write(race);
          race = 1;
          rwLock.ReleaseWriterLock();
          Console.Write(race);
        }).Start();
      }
    }
  }
}

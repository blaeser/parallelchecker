using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ReaderWriter {
    public static void Main() {
      const int N = 10;
      var rwLock = new ReaderWriterLockSlim();
      var race = 0;
      for (int index = 0; index < N; index++) {
        new Thread(() => {
          rwLock.EnterReadLock();
          race = 2;
          rwLock.ExitReadLock();
          rwLock.EnterUpgradeableReadLock();
          Console.WriteLine(race);
          rwLock.EnterWriteLock();
          race = 1;
          rwLock.ExitWriteLock();
          rwLock.ExitUpgradeableReadLock();
        }).Start();
      }
    }
  }
}

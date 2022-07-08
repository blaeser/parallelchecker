using System;
using System.Threading;

namespace ParallelChecker._Test {
  class ReaderUpgradeSlimRaceTest {
    public static void Main() {
      const int N = 10;
      var rwLock = new ReaderWriterLockSlim();
      var race = 0;
      for (int index = 0; index < N; index++) {
        new Thread(() =>
        {
          rwLock.EnterReadLock();
          Console.Write(race);
          rwLock.ExitReadLock();
          rwLock.EnterUpgradeableReadLock();
          Console.Write(race++);
          rwLock.ExitUpgradeableReadLock();
        }).Start();
      }
    }
  }
}

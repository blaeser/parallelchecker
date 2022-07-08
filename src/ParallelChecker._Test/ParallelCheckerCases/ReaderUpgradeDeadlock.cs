using System.Threading;

namespace ParallelChecker._Test {
  class ReaderUpgradeDeadlock {
    public static void Main() {
      const int N = 3;
      var rwLock = new ReaderWriterLock();
      for (int index = 0; index < N; index++) {
        new Thread(() =>
        {
          rwLock.AcquireReaderLock(int.MaxValue);
          rwLock.AcquireWriterLock(int.MaxValue);
          rwLock.ReleaseWriterLock();
          rwLock.ReleaseReaderLock();
        }).Start();
      }
    }
  }
}

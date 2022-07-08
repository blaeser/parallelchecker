using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    Task.Run(() => { using (var test = new UnsafeDisposable()) { } });
    Task.Run(() => { using (var test = new UnsafeDisposable()) { } });
  }

  class UnsafeDisposable : IDisposable {
    private static int _disposed = 0;

    public void Dispose() {
      ++_disposed;
    }
  }
}

using System;

namespace ParallelChecker._Test {
  class AnyTypeDeconstruction {
    public static void Main() {
      using (var instance = new OneDisposable()) {
      }
    }
  }

  class OneDisposable : IDisposable {
    public void Dispose() {
      Console.WriteLine("disposed");
    }
  }
}
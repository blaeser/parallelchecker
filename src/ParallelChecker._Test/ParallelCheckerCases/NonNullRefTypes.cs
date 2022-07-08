using System;
using System.Threading.Tasks;

namespace CSharp8Null {
  class Program {
    static void Main() {
      string? x = null;
      if (x == null) {
        Task.Run(() => x = "Hello");
      }
      Console.Write(x);
      Console.WriteLine(x?.Length);
      x ??= "";
      Console.Write(x!.Length);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

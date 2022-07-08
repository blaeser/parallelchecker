using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestContinue {
  class Program {
    static void Main(string[] args) {
      var task = Task.Run(() => {
        var count = 0;
        ++count;
      });
      var continuation = task.ContinueWith(t => t == task);
      continuation.Wait();
    }
  }
}

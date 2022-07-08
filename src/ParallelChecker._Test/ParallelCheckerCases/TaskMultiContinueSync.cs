using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace TestContinue {
  class Program {
    static void Main(string[] args) {
      var dictionary = new ConcurrentDictionary<Task, int>();
      var task = Task.Run(() => {
        var count = 0;
        ++count;
      });
      dictionary.TryAdd(task, 0);

      var continuation = Task.Factory.ContinueWhenAll(new[] { task }, t => {
        dictionary.TryRemove(task, out var unused);
      });
      continuation.Wait();
    }
  }
}

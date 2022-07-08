using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestContinue {
  class Program {
    static void Main(string[] args) {
      var done = false;
      var task = new Task(() => {
        Thread.Sleep(TimeSpan.FromSeconds(1));
        done = true;
      });
      var follow = task.ContinueWith(x => { });
      task.Start();

      follow.Wait();
      if (!done) {
        Console.WriteLine("The operation should have completed");
      }
    }
  }
}

using System;
using System.Collections;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class SynchronizedWrapper {
    static void Main(string[] args) {
      var x = Stack.Synchronized(new Stack());
      Task.Run(() =>
      {
        x.Push(1);
      });
      x.Push(2);
      Console.WriteLine(x.Count);
    }
  }
}

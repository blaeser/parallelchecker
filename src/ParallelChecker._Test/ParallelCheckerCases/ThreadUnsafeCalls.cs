using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ThreadUnsafeCallTest {
    static void Main(string[] args) {
      var x = new List<int>();
      Task.Run(() =>
      {
        x.Add(1);
      });
      Console.Write(x.Contains(1));
    }
  }
}

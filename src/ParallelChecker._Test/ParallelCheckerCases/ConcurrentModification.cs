using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ConcurrentModificationTest {
    static void Main(string[] args) {
      var x = new List<int>();
      Task.Run(() =>
      {
        x.Add(1);
      });
      foreach (var item in x) {
        Console.Write(item);
      }
    }
  }
}

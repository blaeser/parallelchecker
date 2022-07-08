using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class DelegateTypeTest {
    static void Main(string[] args) {
      var del = (object)(ThreadStart)Run;
      if (del is ThreadStart) {
        var race = 0;
        Task.Run(() => race = 1);
        Console.Write(race);
      }
    }

    static void Run() {

    }
  }
}

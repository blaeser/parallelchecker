using System;
using System.Threading;

namespace ConsoleApp13 {
  class Program {
    static void Main(string[] args) {
      Monitor.Wait("abc");
      Console.WriteLine("Hello World!");
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

using System;
using System.Threading;

namespace ConsoleApp13 {
  class Program {
    static void Main(string[] args) {
      Monitor.Enter("abc");
      Console.WriteLine("Hello World!");
      Monitor.Exit("abc");
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

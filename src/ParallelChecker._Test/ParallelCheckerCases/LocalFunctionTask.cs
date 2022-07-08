using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class LocalFunctionTask {
    static void Main(string[] args) {
      int x = 0;
      void Local() {
        x++;
        int y = 0;
        Task.Run(() => y++);
        Console.WriteLine(y);
      }
      Task.Run(Local);
      Console.WriteLine(x);
    }
  }
}

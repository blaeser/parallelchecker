using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestContinue {
  class Program {
    static void Main(string[] args) {
      var counter1 = 0;
      var counter2 = 0;
      var task1 = Task.Run(() => ++counter1);
      var task2 = Task.Run(() => ++counter2);

      if(Task.WaitAny(task1, task2) == 0) {
        ++counter1;
      } else {
        ++counter2;
      }
    }
  }
}

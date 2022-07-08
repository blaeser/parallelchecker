using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class TaskResult {
    static void Main() {
      var task = Task.Run(() => 3);
      if (task.Result == 3) {
        var expectedRace = 1;
        Task.Run(() => expectedRace = 2);
        Console.Write(expectedRace);
      } else {
        var noRace = 1;
        Task.Run(() => noRace = 2);
        Console.Write(noRace);
      }
    }
  }
}

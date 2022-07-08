using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      int count = 5;
      string label = "Colors used in the map";
      var pair = (count, label);
      var pair2 = pair;
      if (pair.count == 5) {
        Task.Run(() => pair.count++);
      }
      Console.Write(pair.count);
      Console.Write(pair2.count);
      Console.Write(pair.label);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

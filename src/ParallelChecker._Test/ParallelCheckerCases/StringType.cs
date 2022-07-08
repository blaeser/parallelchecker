using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class StringType {
    static void Main() {
      var x = "Test";
      var y = x + "123";
      var z = y + '!';
      if (z == "Test123!") {
        Task.Run(() => z = "");
        Console.Write(z);
      }
      x = null;
      y = null;
      if (x + y == "") {
        Task.Run(() => x = "");
        Console.Write(x);
      }
      var b = x + y != "";
    }
  }
}

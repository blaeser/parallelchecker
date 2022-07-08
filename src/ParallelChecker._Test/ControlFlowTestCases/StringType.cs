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
      }
      x = null;
    }
  }
}

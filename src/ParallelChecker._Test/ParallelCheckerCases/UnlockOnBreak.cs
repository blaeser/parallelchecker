using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      var instance = new object();
      while (true) {
        lock (instance) {
          break;
        }
      }
      Task.Run(() => { lock (instance) { } }).Wait();
      var race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

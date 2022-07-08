using System;

namespace ParallelChecker._Test {
  class ConditionalExpression {
    public static void Main() {
      var x = 1;
      var y = 2;
      var z = x < y ? x : y;
      Console.WriteLine(z);
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

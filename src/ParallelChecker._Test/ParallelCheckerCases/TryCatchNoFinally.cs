using System;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      try {
        throw new Exception();
      } catch (Exception) {

      }
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

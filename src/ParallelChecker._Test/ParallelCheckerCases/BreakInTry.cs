using System;

namespace ParallelChecker._Test {
  class ExceptionTest {

    public static void Main() {
      while (true) {
        try {
          break;
        } catch (Exception) {
        }
      }
      throw new NullReferenceException();
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

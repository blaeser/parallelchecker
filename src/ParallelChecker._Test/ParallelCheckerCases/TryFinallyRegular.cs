using System;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      try {
        Console.WriteLine("TRY");
      } catch (DivideByZeroException) {
        Console.WriteLine("CATCH 1");
      } catch (Exception) {
        Console.WriteLine("CATCH 2");
      } finally {
        Console.WriteLine("FINALLY");
      }
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

using System;

namespace ParallelChecker._Test {
  class ExceptionTest {
    public static void Main() {
      try {
        Console.WriteLine("TRY");
      } catch (DivideByZeroException e1) {
        Console.WriteLine("CATCH 1");
      } catch (Exception e2) {
        Console.WriteLine("CATCH 2");
      } finally {
        Console.WriteLine("FINALLY");
      }
    }
  }
}

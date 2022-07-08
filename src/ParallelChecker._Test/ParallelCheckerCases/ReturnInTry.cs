using System;

namespace ParallelChecker._Test {
  class ReturnInTry {
    static void Main(string[] args) {
      Test1();
      Test2();
      System.Threading.Tasks.Task.Run(() => { });
    }

    static int Test1() {
      try {
        return 0;
      } finally {
        try {
          Console.Write("Test");
        } catch (Exception) {
        }
      }
    }

    static int Test2() {
      try {
        return 0;
      } finally {
        try {
          Console.Write("Test");
          throw new Exception();
        } catch (Exception) {
        }
      }
    }
  }
}

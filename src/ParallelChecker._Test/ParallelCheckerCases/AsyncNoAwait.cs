using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class AsyncNoAwait {
    private static int test;

    public static void Main() {
      Test();
      System.Threading.Tasks.Task.Run(() => { });
    }

    private static async Task Test() {
    }
  }
}

using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class AsyncNoResult {
    private static int test;

    public static void Main() {
      Test().Wait();
    }

    private static async Task Test() {
      await Task.Run(() => { });
    }
  }
}

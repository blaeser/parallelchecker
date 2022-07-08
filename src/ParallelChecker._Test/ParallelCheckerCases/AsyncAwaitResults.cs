using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class AsyncAwaitResults {
    static int race;

    public static void Main() {
      var task = ComputeAsync();
      race = 3;
      if (task.Result == 33) {
        race = 4;
      }
    }

    static async Task<int> ComputeAsync() {
      race = 1;
      if (await Task.Run(() => { return 42; }) == 42) {
        race = 2;
      }
      return 33;
    }
  }
}

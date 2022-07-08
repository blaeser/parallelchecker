using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class SimpleAsyncAwait {
    static int race;

    public static void Main() {
      ComputeAsync();
      race = 3;
    }

    static async void ComputeAsync() {
      race = 1;
      await Task.Run(() => { });
      race = 2;
    }
  }
}

using System.Threading;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      var forks = new SemaphoreSlim[5];
      for (int i = 0; i < forks.Length; i++) {
        forks[i] = new SemaphoreSlim(1);
      }
      for (int count = 0; count < forks.Length; count++) {
        var id = count;
        new Thread(() => {
          var left = id;
          var right = (id + 1) % forks.Length;
          forks[left].Wait();
          forks[right].Wait();
          forks[right].Release();
          forks[left].Release();
        }).Start();
      }
    }
  }
}

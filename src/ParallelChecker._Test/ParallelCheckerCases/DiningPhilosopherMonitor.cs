using System.Threading;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      var forks = new object[5];
      for (int i = 0; i < forks.Length; i++) {
        forks[i] = new object();
      }
      for (int count = 0; count < forks.Length; count++) {
        var id = count;
        new Thread(() => {
          var left = id;
          var right = (id + 1) % forks.Length;
          Monitor.Enter(forks[left]);
          Monitor.Enter(forks[right]);
          Monitor.Exit(forks[right]);
          Monitor.Exit(forks[left]);
        }).Start();
      }
    }
  }
}

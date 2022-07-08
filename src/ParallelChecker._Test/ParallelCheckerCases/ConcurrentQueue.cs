using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

class ConcurrentCollectionTest {
  public static void Main() {
    int noRace = 0;
    var bag = new ConcurrentQueue<Task>();
    for (int i = 0; i < 10; i++) {
      bag.Enqueue(Task.Run(() => Console.Write(noRace)));
    }
    foreach (var item in bag) {
      item.Wait();
    }
    noRace = 1;
  }
}

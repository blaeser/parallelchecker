using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

class ConcurrentCollectionTest {
  public static void Main() {
    int noRace = 0;
    var bag = new ConcurrentBag<Task>();
    for (int i = 0; i < 10; i++) {
      bag.Add(Task.Run(() => Console.Write(noRace)));
    }
    foreach (var item in bag) {
      item.Wait();
    }
    noRace = 1;
  }
}

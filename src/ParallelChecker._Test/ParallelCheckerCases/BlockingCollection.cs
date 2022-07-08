using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

class BlockingCollectionTest {
  public static void Main() {
    var race = 1;
    var buffer = new BlockingCollection<int>(1);
    Task.Run(() => {
      for (int i = 0; i < 10; i++) {
        buffer.Add(i);
      }
      race = 2;
    });
    Task.Run(() =>
    {
      for (int i = 0; i < 10; i++) {
        buffer.Take();
      }
      Console.Write(race);
    });
  }
}

using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class EventsResult {
    private delegate double Operation(int value);

    private event Operation functions;

    private int race;

    public void Run() {
      Operation del = x => race = x;
      functions += del;
      functions += _ => race * race;
      if (functions != null) {
        Parallel.Invoke(() => Console.Write(functions(1)), () => functions(2));
      }
      functions -= del;
    }

    public static void Main() {
      new EventsResult().Run();
    }
  }
}

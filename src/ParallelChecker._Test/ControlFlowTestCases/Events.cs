using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Events {
    private delegate void Operation(int value);

    private event Operation actions;

    private int race;

    public void Run() {
      Operation del = x => race = x;
      actions += del;
      actions += _ => Console.Write(race);
      if (actions != null) {
        Parallel.Invoke(() => actions(1), () => actions(2));
      }
      actions -= del;
    }

    public static void Main() {
      new Events().Run();
    }
  }
}

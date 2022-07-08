using System;
using System.Threading;

class ThreadStartArgument {
  static int race;

  public static void Main() {
    new Thread(TaskLogic).Start(42);
    Console.Write(race);
  }

  static void TaskLogic(object state) {
    if ((int)state == 42) {
      race = (int)state;
    }
  }
}

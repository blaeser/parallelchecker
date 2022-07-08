using System;
using System.Threading;

class ThreadPoolQueue {
  static int race;

  public static void Main() {
    ThreadPool.QueueUserWorkItem(TaskLogic, 42);
    Console.Write(race);
  }

  static void TaskLogic(object state) {
    if ((int)state == 42) {
      race = (int)state;
    }
  }
}

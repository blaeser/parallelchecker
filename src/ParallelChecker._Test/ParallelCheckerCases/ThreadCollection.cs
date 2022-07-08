using System;
using System.Collections.Generic;
using System.Threading;

class Test {
  public static void Main() {
    int race = 0;
    var list = new List<Thread>();
    for (int i = 0; i < 10; i++) {
      var thread = new Thread(() => Console.Write(race));
      list.Add(thread);
      thread.Start();
    }
    race = 1;
    foreach (var thread in list) {
      thread.Join();
    }
    race = 1;
  }
}

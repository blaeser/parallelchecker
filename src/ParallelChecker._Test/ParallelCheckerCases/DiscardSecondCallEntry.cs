using System;
using System.Threading;

namespace ParallelChecker._Test {

  class Test {
    private int race = 0;

    public void Init() {
      new Thread(() => race = 1).Start();
    }

    public int Do() {
      Init();
      return race;
    }
  }

  class Interfaces {
    public static void Main() {

    }
  }
}

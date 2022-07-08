using System;
using System.Threading;

namespace ParallelChecker._Test {
  class Singleton {
    private static Singleton instance;
    private static object locker = new object();
    private int[] data;

    private Singleton() {
      data = new int[100];
    }

    public static Singleton Instance {
      get {
        if (instance == null) {
          lock (locker) {
            if (instance == null) {
              instance = new Singleton();
            }
          }
        }
        return instance;
      }
    }
  }

  class Program {
    static void Main(string[] args) {
      new Thread(() => Console.WriteLine(Singleton.Instance)).Start();
      new Thread(() => Console.WriteLine(Singleton.Instance)).Start();
    }
  }
}

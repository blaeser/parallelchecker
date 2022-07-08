using System;

namespace ParallelChecker._Test {
  class SwitchGotos {
    public static void Main() {
      int i = new Random().Next();
      switch (i) {
        case 0:
          i++;
          goto case 1;
        case 1:
          i--;
          goto default;
        case 2:
          goto case 0;
          test:
          break;
        default:
          Console.Write(i);
          goto test;
      }
    }
  }
}

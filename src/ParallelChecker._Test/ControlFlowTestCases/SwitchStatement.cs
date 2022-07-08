using System;

namespace ParallelChecker._Test {
  class SwitchStatement {
    public static void Main() {
      var x = 0;
      switch(x) {
        case 0:
        case 1:
          x++;
          break;
        case 2:
          Console.WriteLine(x);
          break;
        case 3:
          x = 2;
          break;
        default:
          x = 0;
          break;
      }
      var y = x;
      switch(y) {
        case 1: 
          switch(y) {
            case 2:
              Console.WriteLine(x);
              break;
            default:
              x++;
              break;
          }
          break;
        case 2:
        case 3:
          break;
      }
    }
  }
}

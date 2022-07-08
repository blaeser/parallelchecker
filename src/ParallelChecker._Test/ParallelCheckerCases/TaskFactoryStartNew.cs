using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var x = 0;
    Task.Factory.StartNew(() => x = 1);
    Console.Write(x);
  }
}

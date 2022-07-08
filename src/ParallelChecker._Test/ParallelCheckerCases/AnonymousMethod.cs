using System;
using System.Threading.Tasks;

class Test {

  public static void Main() {
    int y = 0;
    var x = Task.Factory.StartNew(delegate { y = 1; });
    Console.Write(y);
  }
}

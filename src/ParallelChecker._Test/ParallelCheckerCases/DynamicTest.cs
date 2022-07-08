using System;
using System.Threading.Tasks;

class Test {
  void Run() { }
}

class DynamicTest {
  public static void Main() {
    dynamic x = new Test();
    x.Run();
    x = 0;
    Task.Run(() => x++);
    Console.Write(x.InvalidCall(x));
  }
}

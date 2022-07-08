using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    string? x = Identity("Test");
    Task.Run(() => x = null);
    Console.WriteLine(x);

    static T Identity<T>(T item) where T : class {
      return item;
    }
  }
}

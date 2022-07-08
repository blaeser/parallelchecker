using System;

class Test {
  public static void Main() {
    var x = new { Name = "Hans", Age = 40 };
    Console.Write(x.Name);
    if (x.Age == 40) {
      throw new Exception();
    }
  }
}

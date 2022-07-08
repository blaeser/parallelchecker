using System;

class Test {
  public static void Main() {
    var name = "Hans";
    int age = 40;
    var x = new { name, age };
    Console.Write(x.name);
    if (x.age == 40) {
      throw new Exception();
    }
  }
}

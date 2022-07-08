global using System;
global using System.Threading.Tasks;

namespace MyProgram;


record Person(string Name, Address Address) {
}

record Address(int Zip, string City) {
}

class Test {
  public static void Main() {
    Person p = new("Hans", new(8000, "Zürich"));
    if (p is Person { Name: "Hans", Address: { Zip: 8000 } }) {
      int race = 0;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
    if (p is Person { Name: "Hans", Address.Zip: 8000 }) {
      int race = 0;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

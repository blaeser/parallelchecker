using System;
using System.Threading.Tasks;

record Person(string Name, int Age);

class Program {
  public static void Main() {
    var p = new Person("Hans", 60);
    var p2 = p with { Age = 61 };
    Console.WriteLine(p2);
    if (p2.Name == "Hans" && p2.Age == 60) {
      int noRace = 1;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
    if (p2.Name == "Hans" && p2.Age == 61 && p.Age == 60) {
      int race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

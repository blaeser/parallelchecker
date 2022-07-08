using System;
using System.Threading.Tasks;

record Person(string Name, int Age);

class Program {
  public static void Main() {
    var p = new Person("Hans", 60);
    if (p.Name == "Hans" && p.Age == 50 || p.Name == "Paul" && p.Age == 60) {
      int noRace = 1;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
    if (p.Name == "Hans" && p.Age == 60) {
      int race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

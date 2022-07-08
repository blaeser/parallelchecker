using System;
using System.Threading.Tasks;

record Person(string Name, int Age);

class Program {
  public static void Main() {
    var p = new Person("Hans", 60);
    var (name, age) = p;
    if (name == "Hans" && age == 50 || name == "Paul" && age == 60) {
      int noRace = 1;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
    if (name == "Hans" && age == 60) {
      int race = 1;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

using System;
using System.Threading.Tasks;

Person p = new("Hans", 40);
Item i = new() { Description = "Computer" };
if (p.Name == "Hans" && p.Age == 40 && i.Description == "Computer") {
  var race = 0;
  Task.Run(() => race++);
  Console.WriteLine(race);
}
if (p.Name == "Peter" || p.Age == 41 && i.Description == null) {
  var noRace = 0;
  Task.Run(() => noRace++);
  Console.WriteLine(noRace);
}

record Person(string Name, int Age);

class Item {
  public string Description { get; set; }
}

using System;
using System.Threading.Tasks;

int? x = 3;
int? y = null;

if (x is not null && y is null) {
  var race = 0;
  Task.Run(() => race++);
  Console.WriteLine(race);
}
if (x is null || y is not null) {
  var noRace = 0;
  Task.Run(() => noRace++);
  Console.WriteLine(noRace);
}

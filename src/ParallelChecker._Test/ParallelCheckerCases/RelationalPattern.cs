using System;
using System.Threading.Tasks;

static bool IsLetter(char c) =>
    c is >= 'a' and <= 'z' or (>= 'A' and <= 'Z');

if (IsLetter('B')) {
  var race = 0;
  Task.Run(() => race++);
  Console.WriteLine(race);
}
if (IsLetter('0')) {
  var noRace = 0;
  Task.Run(() => noRace++);
  Console.WriteLine(noRace);
}

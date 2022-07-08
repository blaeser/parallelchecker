using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    var range1 = ..;
    var range2 = 1..4;
    var range3 = 1..;
    var range4 = ..^1;
    var indexEnd = ^1;
    Index indexStart = 0;
    var test = indexEnd.Equals(^1);
    if (indexEnd.IsFromEnd && indexEnd.Value == 1 && range1.Equals(0..^0) && range2.Start.Equals(1) && range2.End.Equals(4) && range3.Equals(1..^0) && range4.Equals(0..^1)) {
      int race = 0;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
    if (!indexEnd.IsFromEnd || indexEnd.Value != 1 || !range1.Equals(0..^0) || !range2.Start.Equals(1) || !range2.End.Equals(4) || !range3.Equals(1..^0) || !range4.Equals(0..^1)) {
      int noRace = 0;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
  }
}

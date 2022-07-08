using System;
using System.Threading.Tasks;

class Test {
  public static void Main() {
    int[] array = { 1, 2, 3, 4 };
    if (array[..][0] == 1 && array[..][^1] == 4 && array[1..3].Length == 2 && array[1..^1][0] == 2 && array[1..^1][1] == 3) {
      int race = 0;
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
    if (array[..][0] != 1 | array[..][^1] != 4 || array[1..3].Length != 2 || array[1..^1][0] != 2 || array[1..^1][1] != 3) {
      int noRace = 0;
      Task.Run(() => noRace++);
      Console.WriteLine(noRace);
    }
  }
}

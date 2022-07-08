using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Test {
  static int race = 42;
  static bool initialized;

  [ModuleInitializer]
  internal static void Abc() {
    if (race == 42) {
      Task.Run(() => race++);
    }
    Console.WriteLine("Initialized: " + race);
    initialized = true;
  }

  public static void Main() {
    Console.Write("START");
    if (initialized) {
      Console.WriteLine(race);
    }
  }
}

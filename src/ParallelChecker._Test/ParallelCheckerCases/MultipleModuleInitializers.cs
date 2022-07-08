using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

class Test {
  public static int race = 42;
  private static bool initialized;

  [ModuleInitializer]
  internal static void Abc() {
    if (race == 42) {
      Task.Run(() => race++);
    }
    Console.WriteLine("Initialized: " + race);
    initialized = true;
  }

  [ModuleInitializer]
  public static void Def() {
    Console.WriteLine("SECOND INIT");
    race++;
  }

  public static void Main() {
    Console.Write("START");
    if (initialized) {
      Task.Run(() => race++);
      Console.WriteLine(race);
    }
  }
}

class Test2 {
  [ModuleInitializer]
  public static void Run() {
    Console.WriteLine("THIRD INIT: " + Test.race);
  }
}
using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {
  class Test : IDisposable {
    static int race = 0;
    public void Dispose() {
      Task.Run(() => race++);
    }
  }

  class Program {
    static void Main() {
      using var test2 = new Test();
      for (int i = 0; i < 3; i++) {
        using var test = new Test();
        using var file = new System.IO.StreamWriter("WriteLines2.txt");
        if (test != null) {
          throw new Exception();
        }
      }
      var noRace = 0;
      Task.Run(() => noRace++);
      Console.Write(noRace);
    }
  }
}

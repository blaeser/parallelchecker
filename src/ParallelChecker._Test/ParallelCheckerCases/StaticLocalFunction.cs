using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {
  class Program {
    static void Main() {
      static void Test() {
        int race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }
      Test();
    }
  }
}

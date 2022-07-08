using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      Outer(3);
    }

    static void Outer<T>(T x) {
      Local(x);
      void Local(T y) {
        var race = 0;
        Task.Run(() => race = 1);
        Console.WriteLine(race);
      }
    }
  }
}

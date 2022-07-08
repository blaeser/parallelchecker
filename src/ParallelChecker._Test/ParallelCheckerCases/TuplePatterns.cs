using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {


  class Program {
    static void Main() {
      string first = "A";
      string second = "B";
      var result = (first, second) switch
      {
        ("A", "C") => 1,
        ("C", "B") => 2,
        ("A", "B") => 3,
        _ => 0
      };
      if (result == 3) {
        Task.Run(() => result++);
      }
      Console.WriteLine(result);
    }
  }
}

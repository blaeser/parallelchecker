using System;
using System.Threading.Tasks;

namespace NewCSharp8Features {
  class Address {
    public int Zip { get; set; }
    public string State { get; set; }
  }

  class Program {
    static void Main() {
      var location = new Address {
        Zip = 1234,
        State = "CA"
      };
      var result = location switch
      {
        { State: "WA" } => 1,
        { State: "MN" } => 2,
        { State: "CA", Zip: 1234 } => 3,
        // other cases removed for brevity...
        _ => 0
      };
      if (result == 3) {
        Task.Run(() => result++);
      }
      Console.WriteLine(result);
    }
  }
}

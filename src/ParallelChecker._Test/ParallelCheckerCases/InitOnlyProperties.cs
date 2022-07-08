using System;
using System.Threading.Tasks;

namespace ConsoleApp3 {
  class Person {
    public string Name { get; init; }
    public int Age { get; init; }
  }

  class Order {
    private readonly string description;
    private readonly int amount;

    public string Description {
      get => description;
      init => description = value;
    }

    public int Amount {
      get {
        int race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
        return amount;
      }
      init {
        int race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);

        Task.Run(() => value++);
        amount = value;
      }
    }
  }

  class Program {
    public static void Main() {
      var p = new Person {
        Name = "Hans",
        Age = 60
      };
      if (p.Name == "Hans" && p.Age == 60) {
        int race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }

      var o = new Order {
        Description = "Test",
        Amount = 2
      };
      Console.WriteLine(o.Description + " " + o.Amount);
    }
  }
}

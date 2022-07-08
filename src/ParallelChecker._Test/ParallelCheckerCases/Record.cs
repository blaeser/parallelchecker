using System;
using System.Threading.Tasks;

namespace ConsoleApp3 {
  record Person {
    public string Name { get; init; }
    public int Age { get; init; }
  }

  sealed record Student : Person {
    public Student(string name, int age) {
      Name = name;
      Age = age;
    }
  }

  record Order {
    private readonly string description;
    private readonly int amount;

    public Order(string description, int amount) {
      this.description = description;
      this.amount = amount;
    }

    public string Description {
      get => description;
    }

    public int Amount {
      get {
        int race = 0;
        Task.Run(() => race++);
        Console.WriteLine(race);
        return amount;
      }
    }
  }

  class Program {
    public static void Main() {
      var p = new Student("Hans", 60);
      if (p.Name == "Hans" && p.Age == 60) {
        int race = 1;
        Task.Run(() => race++);
        Console.WriteLine(race);
      }

      var o = new Order("Test", 2);
      Console.WriteLine(o.Description + " " + o.Amount);
    }
  }
}

using System;
using System.Threading;

namespace ParallelChecker._Test {
  interface IPerson {
    string Name { get; }
    int Age { get; }
  }

  struct Person : IPerson {
    private string name;
    public int Age { get; }

    public Person(string name, int age) {
      this.name = name;
      Age = age;
    }

    public string Name {
      get { return name; }
      set { SetName(value); }
    }

    public void SetName(string name) {
      this.name = name;
    }
  }

  class Structs {
    private static Person p4;

    public static void Main() {
      Person p1 = new Person();
      Console.WriteLine(p1.Age);
      Person p2 = new Person("Paul", 30);
      Person p3 = p2;
      p2.Name = "Anna";
      if (p3.Name == "Paul") {
        int first = 0;
        new Thread(() => first = 1).Start();
        Console.Write(first);
      }
      p4.SetName("Hans");
      if (p4.Name == "Hans") {
        int second = 0;
        new Thread(() => second = 1).Start();
        Console.Write(second);
      }
      object o = p2;
      IPerson i1 = p2;
      IPerson i2 = p2;
      IPerson i3 = i2;
      if (i1 != i2 && i2 == i3) {
        int third = 0;
        new Thread(() => third = 1).Start();
        Console.Write(third);
      }
      if (o is Person && ((Person)o).Age == 30) {
        int fourth = 0;
        new Thread(() => fourth = 1).Start();
        Console.Write(fourth);
      }
    }
  }
}

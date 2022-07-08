using System;
using System.Threading.Tasks;

Console.WriteLine("Test");
int race = 0;
Task.Run(() => race++);
Console.WriteLine(race);

partial class Program {
  public static void Main() {
    Console.WriteLine("Main");
  }
}
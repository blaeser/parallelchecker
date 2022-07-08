using System;
using System.Threading.Tasks;

class Test {
  static Test() {
    int race = 0;
    void local() {
      Task.Run(() => race++);
    }
    local();
    Console.WriteLine(race);
  }

  public static void Main() {

  }
}

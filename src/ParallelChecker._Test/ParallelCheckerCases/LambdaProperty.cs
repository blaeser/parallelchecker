using System;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Test {
    public int Property1 { get; set; }

    public int Property2 => Property1 + 1;
  }

  public class LambdaProperty {
    public static void Main() {
      Test x = new Test();
      Task.Run(() => x.Property1 = 7);
      Console.WriteLine(x.Property2);
    }
  }
}

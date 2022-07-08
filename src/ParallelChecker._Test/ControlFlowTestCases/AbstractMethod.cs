using System;
using System.Threading.Tasks;

class Test {
  abstract class Base {
    public abstract int GetValue();
  }

  class Sub : Base {
    public override int GetValue() {
      return 1;
    }
  }

  public static void Main() {
    Base b = new Sub();
    if (b.GetValue() == 1) {
      Task.Run(() => b = null);
      Console.Write(b);
    }
  }
}

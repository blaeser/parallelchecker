using System.Threading;

class Test {
  enum EnumType { None = 0, First, Second = 2 }

  public static void Main() {
    var x = EnumType.First;
    if (x == EnumType.First) {
      new Thread(() => x = default(EnumType)).Start();
      x = EnumType.Second;
    }
  }
}
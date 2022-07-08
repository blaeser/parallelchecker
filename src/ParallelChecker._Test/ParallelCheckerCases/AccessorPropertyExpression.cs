using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static int x;

    static int MyValue {
      get => 42;
      set => Task.Run(() => x = value);
    }

    static void Main() {
      if (MyValue == 42) {
        MyValue = 1;
        MyValue = 2;
      }
    }
  }
}

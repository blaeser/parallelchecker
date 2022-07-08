using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    int x;

    int this[int index] {
      get => index;
      set => Task.Run(() => x = value);
    }

    static void Main() {
      var instance = new Program();
      if (instance[42] == 42) {
        instance[1] = 1;
        instance[2] = 2;
      }
    }
  }
}

using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static int race;

    static ref int Property {
      get { return ref race; }
    }

    public static void Main() {
      int[] array = { 0, 1 };
      ref int y = ref Test(array);
      y = 2;
      if (array[0] == 2) {
        Task.Run(() => Property = 100);
      }
      array[0] = 3;
      if (y == 3) {
        Task.Run(() => Property = 100);
      }
      Test(array) = 4;
      if (y == 4) {
        Task.Run(() => Property = 100);
      }
    }

    private static ref int Test(int[] array) {
      return ref array[0];
    }
  }
}

using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main() {
      int x = 0;
      void Test(int y)
      {
        Task.Run(() => x = y);
      }
      Test(1);
      Test(2);
    }
  }
}

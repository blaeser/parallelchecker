using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Program {
    static void Main() {
      int x = 0;
      void Test(int y)
      {
        void Test2()
        {
          int a = 1;
          Task.Run(() => x = y + a);
        }
        Test2();
      }
      Test(1);
      Test(2);
    }
  }
}

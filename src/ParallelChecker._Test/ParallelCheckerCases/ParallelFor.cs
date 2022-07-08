using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ParallelFor {
    const int N = 10;

    static void Main() {
      var array = new int[N];
      Parallel.For(1, N, index => {
        array[index] = array[index - 1];
      });
    }
  }
}

using System.Collections;
using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class ConcurrentModificationTest {
    static void Main(string[] args) {
      var x = new Stack();
      x.Push(0);
      Task.Run(() => {
        x.Push(1);
      });
      x.Peek();
    }
  }
}

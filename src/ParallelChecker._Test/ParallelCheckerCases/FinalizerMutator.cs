using System.Threading.Tasks;

namespace ParallelChecker._Test {
  class Block {
    static int count;

    Block() {
      count++;
    }

    ~Block() {
      count--;
    }

    public static void Main() {
      new Block();
      Task.Run(() => new Block());
    }
  }
}

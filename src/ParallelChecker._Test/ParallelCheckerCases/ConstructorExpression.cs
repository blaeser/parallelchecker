using System.Threading.Tasks;

namespace ParallelChecker._Test {

  class Node {
    private static int race;

    public Node(int x) => Task.Run(() => race = x);
  }

  class Program {
    static void Main() {
      new Node(1);
      new Node(2);
    }
  }
}

using System;
using System.Threading.Tasks;

namespace CheckerDevTest {
  class Program {
    static void Main() {
      var node = new Node();
      if (node.value == 43) {
        Task.Run(() => node = null);
        Console.WriteLine(node);
      }
    }
  }

  class Node {
    public int value;

    public Node() => Test(out int a, out value);

    private void Test(out int x, out int y) {
      x = 42;
      y = 43;
    }
  }
}

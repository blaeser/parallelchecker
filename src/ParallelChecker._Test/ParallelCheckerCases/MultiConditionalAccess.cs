namespace CheckerDevTest {
  class Node {
    public Node next;
    public void Test() { }
  }

  class Program {
    static void Main() {
      Node node = null;
      node?.next?.Test();
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

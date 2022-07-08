namespace ParallelChecker._Test {
  class Test {
    public static Test operator ++(Test x) {
      return x;
    }
  }

  class SByteTest {
    static void Main(string[] args) {
      sbyte b = 1;
      b++;
      var x = new Test();
      x++;
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

namespace ParallelChecker._Test {
  class Program {
    static void Main(string[] args) {
      var y = Test();
      System.Threading.Tasks.Task.Run(() => { });
    }

    static (int, string) Test() {
      return (1, "Test");
    }
  }
}

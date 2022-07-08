namespace ParallelChecker._Test {
  class MemoryBound {
    static void Main() {
      var matrix = new int[1000][];
      for (int i = 0; i < matrix.Length; i++) {
        matrix[i] = new int[10000];
      }
      System.Threading.Tasks.Task.Run(() => { });
    }
  }
}

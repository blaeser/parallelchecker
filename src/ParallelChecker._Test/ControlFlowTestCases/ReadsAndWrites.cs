namespace ParallelChecker._Test {
  class ReadsAndWrites {
    public static void Main() {
      var x = 0;
      x = 1;
      var y = x;
      x = y + 1;
      x++;
    }
  }
}

namespace ParallelChecker._Test {
  class WhileStatement {
    public static void Main() {
      var x = 0;
      while (x < 100) {
        x++;
      }
      while (x < 200) {
        var y = 1;
        while (y < x) {
          y++;
        }
      }
    }
  }
}

namespace ParallelChecker._Test {
  class IfElseStatement {
    public static void Main() {
      var x = 0;
      var y = 1;
      if (x < y) {
        x++;
      } else if (x == y) {
        x = 1;
      } else {
        x = 2;
      }
      if (x == 0) {
        y = x + 3;
        if (y < x) {
          y++;
          if (y == 0) { }
        }
      }
    }
  }
}

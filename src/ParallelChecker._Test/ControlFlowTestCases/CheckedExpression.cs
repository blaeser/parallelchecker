namespace ParallelChecker._Test {
  class Checked {
    public static void Main() {
      int x = 1234567;
      short y = unchecked((short)x);
      short z = checked((short)x);
    }
  }
}

class VoidExpressionBody {
  static int x;

  public static void Main() {
    Test();
  }

  static void Test() => x = 0;
}

class VoidExpressionBody {
  static int x;

  public static void Main() {
    Test();
    System.Threading.Tasks.Task.Run(() => { });
  }

  static void Test() => x = 0;
}

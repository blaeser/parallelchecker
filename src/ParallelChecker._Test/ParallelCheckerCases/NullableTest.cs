class NullableTest {
  public static void Main() {
    int? x = 3;
    int y = (int)x;
    x = (int?)null;
    x = (byte)3;
    System.Threading.Tasks.Task.Run(() => { });
  }
}

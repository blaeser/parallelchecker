class Test {
  class Rect {
    public Rect(int height = 10, int width = 20) { }
  }

  public static void Main() {
    new Rect();
    new Rect(1);
    new Rect(1, 2);
    new Rect(1, width: 2);
    new Rect(width: 2, height: 1);
    System.Threading.Tasks.Task.Run(() => { });
  }
}

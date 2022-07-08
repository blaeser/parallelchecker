class Test {
  class Rect {
    public int height;
    public int Width { get; set; }

    private int _x;

    public int X {
      get { return _x; }
      set { _x = value; }
    }

    public int Y { get; }

    public Rect(int y) {
      Y = y;
    }
  }

  public static void Main() {
    var r = new Rect(4) { height = 1, Width = 2, X = 3 };
  }
}

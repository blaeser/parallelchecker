public class Program {
  static void Main() {
    new SomethingDisposable(false);
  }
}

public class SomethingDisposable {
  private bool _flag;

  public SomethingDisposable(bool flag) {
    _flag = flag;
  }

  ~SomethingDisposable() {
    _flag = true;
  }
}

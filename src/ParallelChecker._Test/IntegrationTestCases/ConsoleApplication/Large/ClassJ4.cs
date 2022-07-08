namespace Large {
  internal class ClassJ4 {
    private readonly ClassJ5 _next = new ClassJ5();
    private int nofCalls = 0;

    public void F() {
      nofCalls++;
      G();
    }

    private void G() {
      _next.F();
    }
  }
}

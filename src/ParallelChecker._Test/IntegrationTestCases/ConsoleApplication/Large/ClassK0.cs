namespace Large {
  internal class ClassK0 {
    private readonly ClassK1 _next = new ClassK1();
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

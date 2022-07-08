namespace Large {
  internal class ClassJ9 {
    private readonly ClassK0 _next = new ClassK0();
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

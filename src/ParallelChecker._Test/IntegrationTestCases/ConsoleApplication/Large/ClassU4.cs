namespace Large {
  internal class ClassU4 {
    private readonly ClassU5 _next = new ClassU5();
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

namespace Large {
  internal class ClassU0 {
    private readonly ClassU1 _next = new ClassU1();
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

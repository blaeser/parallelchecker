namespace Large {
  internal class ClassU3 {
    private readonly ClassU4 _next = new ClassU4();
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

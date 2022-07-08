namespace Large {
  internal class ClassI4 {
    private readonly ClassI5 _next = new ClassI5();
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

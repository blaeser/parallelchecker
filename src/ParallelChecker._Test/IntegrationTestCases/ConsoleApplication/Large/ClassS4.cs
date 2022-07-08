namespace Large {
  internal class ClassS4 {
    private readonly ClassS5 _next = new ClassS5();
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

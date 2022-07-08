namespace Large {
  internal class ClassK9 {
    private readonly ClassL0 _next = new ClassL0();
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

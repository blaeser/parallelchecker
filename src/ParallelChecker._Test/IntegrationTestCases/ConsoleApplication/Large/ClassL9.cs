namespace Large {
  internal class ClassL9 {
    private readonly ClassM0 _next = new ClassM0();
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

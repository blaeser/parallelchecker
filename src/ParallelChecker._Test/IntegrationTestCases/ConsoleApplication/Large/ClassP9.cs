namespace Large {
  internal class ClassP9 {
    private readonly ClassQ0 _next = new ClassQ0();
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

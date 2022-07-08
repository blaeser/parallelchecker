namespace Large {
  internal class ClassV9 {
    private readonly ClassW0 _next = new ClassW0();
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

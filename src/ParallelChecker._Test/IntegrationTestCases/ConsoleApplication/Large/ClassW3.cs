namespace Large {
  internal class ClassW3 {
    private readonly ClassW4 _next = new ClassW4();
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

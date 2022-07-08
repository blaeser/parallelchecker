namespace Large {
  internal class ClassW4 {
    private readonly ClassW5 _next = new ClassW5();
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

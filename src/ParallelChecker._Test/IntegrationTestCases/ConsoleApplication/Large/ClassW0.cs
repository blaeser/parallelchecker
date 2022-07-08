namespace Large {
  internal class ClassW0 {
    private readonly ClassW1 _next = new ClassW1();
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

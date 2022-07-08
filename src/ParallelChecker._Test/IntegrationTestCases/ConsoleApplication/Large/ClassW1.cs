namespace Large {
  internal class ClassW1 {
    private readonly ClassW2 _next = new ClassW2();
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

namespace Large {
  internal class ClassW2 {
    private readonly ClassW3 _next = new ClassW3();
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

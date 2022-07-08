namespace Large {
  internal class ClassW5 {
    private readonly ClassW6 _next = new ClassW6();
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

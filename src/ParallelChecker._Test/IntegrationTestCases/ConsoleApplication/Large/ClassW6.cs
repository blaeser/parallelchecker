namespace Large {
  internal class ClassW6 {
    private readonly ClassW7 _next = new ClassW7();
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

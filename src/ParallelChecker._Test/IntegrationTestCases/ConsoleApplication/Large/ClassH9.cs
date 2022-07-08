namespace Large {
  internal class ClassH9 {
    private readonly ClassI0 _next = new ClassI0();
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

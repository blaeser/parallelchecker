namespace Large {
  internal class ClassI0 {
    private readonly ClassI1 _next = new ClassI1();
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

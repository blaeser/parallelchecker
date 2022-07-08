namespace Large {
  internal class ClassI6 {
    private readonly ClassI7 _next = new ClassI7();
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

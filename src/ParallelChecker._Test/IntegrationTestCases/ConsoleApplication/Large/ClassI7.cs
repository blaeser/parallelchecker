namespace Large {
  internal class ClassI7 {
    private readonly ClassI8 _next = new ClassI8();
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

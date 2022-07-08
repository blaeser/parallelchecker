namespace Large {
  internal class ClassI3 {
    private readonly ClassI4 _next = new ClassI4();
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

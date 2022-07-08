namespace Large {
  internal class ClassI5 {
    private readonly ClassI6 _next = new ClassI6();
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

namespace Large {
  internal class ClassF9 {
    private readonly ClassG0 _next = new ClassG0();
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

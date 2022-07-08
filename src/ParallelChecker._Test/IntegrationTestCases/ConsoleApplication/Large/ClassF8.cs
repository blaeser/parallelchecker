namespace Large {
  internal class ClassF8 {
    private readonly ClassF9 _next = new ClassF9();
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

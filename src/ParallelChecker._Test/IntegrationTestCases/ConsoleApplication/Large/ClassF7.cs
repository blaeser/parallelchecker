namespace Large {
  internal class ClassF7 {
    private readonly ClassF8 _next = new ClassF8();
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

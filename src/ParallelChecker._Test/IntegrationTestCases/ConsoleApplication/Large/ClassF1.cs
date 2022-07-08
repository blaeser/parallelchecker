namespace Large {
  internal class ClassF1 {
    private readonly ClassF2 _next = new ClassF2();
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

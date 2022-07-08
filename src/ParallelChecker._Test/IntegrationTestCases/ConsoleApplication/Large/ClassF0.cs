namespace Large {
  internal class ClassF0 {
    private readonly ClassF1 _next = new ClassF1();
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

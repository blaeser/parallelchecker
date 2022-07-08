namespace Large {
  internal class ClassF2 {
    private readonly ClassF3 _next = new ClassF3();
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

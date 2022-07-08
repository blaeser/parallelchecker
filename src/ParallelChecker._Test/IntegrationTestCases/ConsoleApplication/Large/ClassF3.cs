namespace Large {
  internal class ClassF3 {
    private readonly ClassF4 _next = new ClassF4();
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

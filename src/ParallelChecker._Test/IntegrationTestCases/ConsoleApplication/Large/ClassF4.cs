namespace Large {
  internal class ClassF4 {
    private readonly ClassF5 _next = new ClassF5();
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

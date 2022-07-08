namespace Large {
  internal class ClassF5 {
    private readonly ClassF6 _next = new ClassF6();
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

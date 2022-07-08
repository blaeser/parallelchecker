namespace Large {
  internal class ClassK4 {
    private readonly ClassK5 _next = new ClassK5();
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

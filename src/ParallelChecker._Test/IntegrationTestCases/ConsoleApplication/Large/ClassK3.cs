namespace Large {
  internal class ClassK3 {
    private readonly ClassK4 _next = new ClassK4();
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

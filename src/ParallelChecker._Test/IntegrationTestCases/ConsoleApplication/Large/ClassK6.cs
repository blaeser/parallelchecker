namespace Large {
  internal class ClassK6 {
    private readonly ClassK7 _next = new ClassK7();
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

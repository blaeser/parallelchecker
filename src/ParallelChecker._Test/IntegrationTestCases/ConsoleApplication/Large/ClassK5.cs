namespace Large {
  internal class ClassK5 {
    private readonly ClassK6 _next = new ClassK6();
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

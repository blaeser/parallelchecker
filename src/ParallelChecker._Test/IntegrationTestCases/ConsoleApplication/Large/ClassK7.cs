namespace Large {
  internal class ClassK7 {
    private readonly ClassK8 _next = new ClassK8();
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

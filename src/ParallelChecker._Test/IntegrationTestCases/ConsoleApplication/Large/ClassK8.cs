namespace Large {
  internal class ClassK8 {
    private readonly ClassK9 _next = new ClassK9();
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

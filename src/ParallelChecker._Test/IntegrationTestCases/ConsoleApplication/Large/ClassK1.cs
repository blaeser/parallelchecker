namespace Large {
  internal class ClassK1 {
    private readonly ClassK2 _next = new ClassK2();
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

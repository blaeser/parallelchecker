namespace Large {
  internal class ClassK2 {
    private readonly ClassK3 _next = new ClassK3();
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

namespace Large {
  internal class ClassX1 {
    private readonly ClassX2 _next = new ClassX2();
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

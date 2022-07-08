namespace Large {
  internal class ClassX2 {
    private readonly ClassX3 _next = new ClassX3();
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

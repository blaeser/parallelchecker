namespace Large {
  internal class ClassL2 {
    private readonly ClassL3 _next = new ClassL3();
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

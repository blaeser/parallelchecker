namespace Large {
  internal class ClassL1 {
    private readonly ClassL2 _next = new ClassL2();
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

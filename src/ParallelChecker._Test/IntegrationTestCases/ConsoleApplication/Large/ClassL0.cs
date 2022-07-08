namespace Large {
  internal class ClassL0 {
    private readonly ClassL1 _next = new ClassL1();
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

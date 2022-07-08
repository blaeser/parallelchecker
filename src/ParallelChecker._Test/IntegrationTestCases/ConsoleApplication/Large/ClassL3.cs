namespace Large {
  internal class ClassL3 {
    private readonly ClassL4 _next = new ClassL4();
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

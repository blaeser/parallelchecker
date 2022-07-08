namespace Large {
  internal class ClassL4 {
    private readonly ClassL5 _next = new ClassL5();
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

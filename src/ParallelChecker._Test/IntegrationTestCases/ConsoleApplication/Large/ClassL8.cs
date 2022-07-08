namespace Large {
  internal class ClassL8 {
    private readonly ClassL9 _next = new ClassL9();
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

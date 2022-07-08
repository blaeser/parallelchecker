namespace Large {
  internal class ClassL6 {
    private readonly ClassL7 _next = new ClassL7();
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

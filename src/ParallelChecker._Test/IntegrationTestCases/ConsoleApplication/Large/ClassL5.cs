namespace Large {
  internal class ClassL5 {
    private readonly ClassL6 _next = new ClassL6();
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

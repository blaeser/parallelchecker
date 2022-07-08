namespace Large {
  internal class ClassL7 {
    private readonly ClassL8 _next = new ClassL8();
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

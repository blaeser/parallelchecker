namespace Large {
  internal class ClassV8 {
    private readonly ClassV9 _next = new ClassV9();
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

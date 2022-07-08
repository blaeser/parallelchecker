namespace Large {
  internal class ClassV7 {
    private readonly ClassV8 _next = new ClassV8();
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

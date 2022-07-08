namespace Large {
  internal class ClassU9 {
    private readonly ClassV0 _next = new ClassV0();
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

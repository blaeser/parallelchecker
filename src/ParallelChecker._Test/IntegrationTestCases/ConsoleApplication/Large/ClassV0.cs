namespace Large {
  internal class ClassV0 {
    private readonly ClassV1 _next = new ClassV1();
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

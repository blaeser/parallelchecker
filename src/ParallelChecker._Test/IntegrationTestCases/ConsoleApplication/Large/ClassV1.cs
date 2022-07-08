namespace Large {
  internal class ClassV1 {
    private readonly ClassV2 _next = new ClassV2();
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

namespace Large {
  internal class ClassV2 {
    private readonly ClassV3 _next = new ClassV3();
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

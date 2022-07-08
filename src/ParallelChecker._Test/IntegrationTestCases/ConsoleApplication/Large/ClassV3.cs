namespace Large {
  internal class ClassV3 {
    private readonly ClassV4 _next = new ClassV4();
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

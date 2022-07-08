namespace Large {
  internal class ClassV5 {
    private readonly ClassV6 _next = new ClassV6();
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

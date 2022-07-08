namespace Large {
  internal class ClassV4 {
    private readonly ClassV5 _next = new ClassV5();
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

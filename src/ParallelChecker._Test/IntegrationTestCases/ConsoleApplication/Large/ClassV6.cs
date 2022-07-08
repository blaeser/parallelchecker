namespace Large {
  internal class ClassV6 {
    private readonly ClassV7 _next = new ClassV7();
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

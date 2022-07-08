namespace Large {
  internal class ClassG9 {
    private readonly ClassH0 _next = new ClassH0();
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

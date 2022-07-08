namespace Large {
  internal class ClassT9 {
    private readonly ClassU0 _next = new ClassU0();
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

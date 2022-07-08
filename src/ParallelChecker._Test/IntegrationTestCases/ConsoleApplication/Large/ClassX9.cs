namespace Large {
  internal class ClassX9 {
    private readonly ClassY0 _next = new ClassY0();
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

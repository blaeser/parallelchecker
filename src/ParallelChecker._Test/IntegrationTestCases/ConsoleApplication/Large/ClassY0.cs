namespace Large {
  internal class ClassY0 {
    private readonly ClassY1 _next = new ClassY1();
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

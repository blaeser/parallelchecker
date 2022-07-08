namespace Large {
  internal class ClassY1 {
    private readonly ClassY2 _next = new ClassY2();
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

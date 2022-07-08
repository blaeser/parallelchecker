namespace Large {
  internal class ClassY2 {
    private readonly ClassY3 _next = new ClassY3();
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

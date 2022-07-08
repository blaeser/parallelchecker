namespace Large {
  internal class ClassY3 {
    private readonly ClassY4 _next = new ClassY4();
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

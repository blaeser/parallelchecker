namespace Large {
  internal class ClassY4 {
    private readonly ClassY5 _next = new ClassY5();
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

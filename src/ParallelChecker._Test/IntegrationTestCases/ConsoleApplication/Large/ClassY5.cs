namespace Large {
  internal class ClassY5 {
    private readonly ClassY6 _next = new ClassY6();
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

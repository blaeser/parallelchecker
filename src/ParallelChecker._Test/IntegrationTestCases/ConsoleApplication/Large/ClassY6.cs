namespace Large {
  internal class ClassY6 {
    private readonly ClassY7 _next = new ClassY7();
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

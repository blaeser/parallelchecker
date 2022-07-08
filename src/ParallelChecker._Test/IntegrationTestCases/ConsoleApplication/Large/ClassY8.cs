namespace Large {
  internal class ClassY8 {
    private readonly ClassY9 _next = new ClassY9();
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

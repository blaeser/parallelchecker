namespace Large {
  internal class ClassY9 {
    private readonly ClassZ0 _next = new ClassZ0();
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

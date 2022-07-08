namespace Large {
  internal class ClassY7 {
    private readonly ClassY8 _next = new ClassY8();
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

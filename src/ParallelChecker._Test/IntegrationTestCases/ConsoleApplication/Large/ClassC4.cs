namespace Large {
  internal class ClassC4 {
    private readonly ClassC5 _next = new ClassC5();
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

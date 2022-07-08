namespace Large {
  internal class ClassC3 {
    private readonly ClassC4 _next = new ClassC4();
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

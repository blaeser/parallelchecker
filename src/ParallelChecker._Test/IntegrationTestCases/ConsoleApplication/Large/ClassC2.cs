namespace Large {
  internal class ClassC2 {
    private readonly ClassC3 _next = new ClassC3();
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

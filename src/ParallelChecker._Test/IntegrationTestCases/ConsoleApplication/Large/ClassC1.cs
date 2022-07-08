namespace Large {
  internal class ClassC1 {
    private readonly ClassC2 _next = new ClassC2();
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

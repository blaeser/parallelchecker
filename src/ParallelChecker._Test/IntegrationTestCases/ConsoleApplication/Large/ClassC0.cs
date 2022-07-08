namespace Large {
  internal class ClassC0 {
    private readonly ClassC1 _next = new ClassC1();
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

namespace Large {
  internal class ClassS1 {
    private readonly ClassS2 _next = new ClassS2();
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

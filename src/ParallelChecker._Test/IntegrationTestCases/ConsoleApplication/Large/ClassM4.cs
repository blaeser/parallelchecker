namespace Large {
  internal class ClassM4 {
    private readonly ClassM5 _next = new ClassM5();
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

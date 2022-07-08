namespace Large {
  internal class ClassM3 {
    private readonly ClassM4 _next = new ClassM4();
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

namespace Large {
  internal class ClassM2 {
    private readonly ClassM3 _next = new ClassM3();
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

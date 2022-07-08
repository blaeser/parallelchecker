namespace Large {
  internal class ClassM1 {
    private readonly ClassM2 _next = new ClassM2();
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

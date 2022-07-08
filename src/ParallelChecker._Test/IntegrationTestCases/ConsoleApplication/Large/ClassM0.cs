namespace Large {
  internal class ClassM0 {
    private readonly ClassM1 _next = new ClassM1();
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

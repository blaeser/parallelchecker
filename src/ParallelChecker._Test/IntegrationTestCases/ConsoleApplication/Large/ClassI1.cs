namespace Large {
  internal class ClassI1 {
    private readonly ClassI2 _next = new ClassI2();
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

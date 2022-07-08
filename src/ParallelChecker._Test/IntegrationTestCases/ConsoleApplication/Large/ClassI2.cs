namespace Large {
  internal class ClassI2 {
    private readonly ClassI3 _next = new ClassI3();
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

namespace Large {
  internal class ClassX0 {
    private readonly ClassX1 _next = new ClassX1();
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

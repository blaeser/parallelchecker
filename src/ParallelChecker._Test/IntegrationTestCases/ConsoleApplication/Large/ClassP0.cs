namespace Large {
  internal class ClassP0 {
    private readonly ClassP1 _next = new ClassP1();
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

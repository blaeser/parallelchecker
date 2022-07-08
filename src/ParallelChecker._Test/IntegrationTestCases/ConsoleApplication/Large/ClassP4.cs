namespace Large {
  internal class ClassP4 {
    private readonly ClassP5 _next = new ClassP5();
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

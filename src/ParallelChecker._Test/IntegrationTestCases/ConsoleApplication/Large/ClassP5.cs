namespace Large {
  internal class ClassP5 {
    private readonly ClassP6 _next = new ClassP6();
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

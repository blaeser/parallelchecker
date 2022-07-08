namespace Large {
  internal class ClassP3 {
    private readonly ClassP4 _next = new ClassP4();
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

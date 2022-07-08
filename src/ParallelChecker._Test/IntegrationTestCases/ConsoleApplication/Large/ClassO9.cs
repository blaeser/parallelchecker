namespace Large {
  internal class ClassO9 {
    private readonly ClassP0 _next = new ClassP0();
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

namespace Large {
  internal class ClassO0 {
    private readonly ClassO1 _next = new ClassO1();
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

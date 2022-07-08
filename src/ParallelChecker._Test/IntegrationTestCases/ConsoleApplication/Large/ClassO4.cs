namespace Large {
  internal class ClassO4 {
    private readonly ClassO5 _next = new ClassO5();
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

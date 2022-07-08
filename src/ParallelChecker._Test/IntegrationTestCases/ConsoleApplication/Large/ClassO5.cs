namespace Large {
  internal class ClassO5 {
    private readonly ClassO6 _next = new ClassO6();
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

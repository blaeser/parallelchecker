namespace Large {
  internal class ClassO3 {
    private readonly ClassO4 _next = new ClassO4();
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

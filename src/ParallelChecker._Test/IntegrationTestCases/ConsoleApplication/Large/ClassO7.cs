namespace Large {
  internal class ClassO7 {
    private readonly ClassO8 _next = new ClassO8();
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

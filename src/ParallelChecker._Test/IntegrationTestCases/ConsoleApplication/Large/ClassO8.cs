namespace Large {
  internal class ClassO8 {
    private readonly ClassO9 _next = new ClassO9();
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

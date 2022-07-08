namespace Large {
  internal class ClassO6 {
    private readonly ClassO7 _next = new ClassO7();
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

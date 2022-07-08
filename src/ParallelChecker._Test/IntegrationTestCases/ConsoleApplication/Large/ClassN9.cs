namespace Large {
  internal class ClassN9 {
    private readonly ClassO0 _next = new ClassO0();
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

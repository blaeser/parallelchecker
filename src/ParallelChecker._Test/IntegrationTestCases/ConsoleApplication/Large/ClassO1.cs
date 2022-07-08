namespace Large {
  internal class ClassO1 {
    private readonly ClassO2 _next = new ClassO2();
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

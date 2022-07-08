namespace Large {
  internal class ClassO2 {
    private readonly ClassO3 _next = new ClassO3();
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

namespace Large {
  internal class ClassC9 {
    private readonly ClassD0 _next = new ClassD0();
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

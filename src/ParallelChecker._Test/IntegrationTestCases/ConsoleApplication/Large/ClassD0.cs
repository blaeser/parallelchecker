namespace Large {
  internal class ClassD0 {
    private readonly ClassD1 _next = new ClassD1();
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

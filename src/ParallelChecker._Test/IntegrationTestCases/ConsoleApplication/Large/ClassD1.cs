namespace Large {
  internal class ClassD1 {
    private readonly ClassD2 _next = new ClassD2();
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

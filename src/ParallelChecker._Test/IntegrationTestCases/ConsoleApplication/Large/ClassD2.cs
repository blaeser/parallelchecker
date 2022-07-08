namespace Large {
  internal class ClassD2 {
    private readonly ClassD3 _next = new ClassD3();
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

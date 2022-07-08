namespace Large {
  internal class ClassD3 {
    private readonly ClassD4 _next = new ClassD4();
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

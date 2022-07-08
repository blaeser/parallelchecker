namespace Large {
  internal class ClassH1 {
    private readonly ClassH2 _next = new ClassH2();
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

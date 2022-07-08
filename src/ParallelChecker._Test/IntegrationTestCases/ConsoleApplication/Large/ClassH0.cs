namespace Large {
  internal class ClassH0 {
    private readonly ClassH1 _next = new ClassH1();
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

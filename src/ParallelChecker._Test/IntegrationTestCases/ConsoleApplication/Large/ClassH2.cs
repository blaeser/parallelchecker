namespace Large {
  internal class ClassH2 {
    private readonly ClassH3 _next = new ClassH3();
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

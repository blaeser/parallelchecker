namespace Large {
  internal class ClassH4 {
    private readonly ClassH5 _next = new ClassH5();
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

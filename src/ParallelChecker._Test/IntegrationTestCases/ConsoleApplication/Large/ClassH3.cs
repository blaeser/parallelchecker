namespace Large {
  internal class ClassH3 {
    private readonly ClassH4 _next = new ClassH4();
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

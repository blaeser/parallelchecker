namespace Large {
  internal class ClassH5 {
    private readonly ClassH6 _next = new ClassH6();
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

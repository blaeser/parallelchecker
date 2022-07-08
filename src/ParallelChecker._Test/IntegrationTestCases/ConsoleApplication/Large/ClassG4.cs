namespace Large {
  internal class ClassG4 {
    private readonly ClassG5 _next = new ClassG5();
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

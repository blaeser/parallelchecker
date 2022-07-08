namespace Large {
  internal class ClassG5 {
    private readonly ClassG6 _next = new ClassG6();
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

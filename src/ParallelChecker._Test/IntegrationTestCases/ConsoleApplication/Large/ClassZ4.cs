namespace Large {
  internal class ClassZ4 {
    private readonly ClassZ5 _next = new ClassZ5();
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

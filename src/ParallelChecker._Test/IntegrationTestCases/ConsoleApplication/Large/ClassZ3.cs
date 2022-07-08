namespace Large {
  internal class ClassZ3 {
    private readonly ClassZ4 _next = new ClassZ4();
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

namespace Large {
  internal class ClassG8 {
    private readonly ClassG9 _next = new ClassG9();
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

namespace Large {
  internal class ClassG2 {
    private readonly ClassG3 _next = new ClassG3();
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

namespace Large {
  internal class ClassG7 {
    private readonly ClassG8 _next = new ClassG8();
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

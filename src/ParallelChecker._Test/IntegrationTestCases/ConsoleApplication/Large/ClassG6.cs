namespace Large {
  internal class ClassG6 {
    private readonly ClassG7 _next = new ClassG7();
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

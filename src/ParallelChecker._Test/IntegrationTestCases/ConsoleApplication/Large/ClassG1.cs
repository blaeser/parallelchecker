namespace Large {
  internal class ClassG1 {
    private readonly ClassG2 _next = new ClassG2();
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

namespace Large {
  internal class ClassG0 {
    private readonly ClassG1 _next = new ClassG1();
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

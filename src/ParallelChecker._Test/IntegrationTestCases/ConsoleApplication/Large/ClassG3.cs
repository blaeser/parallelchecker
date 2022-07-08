namespace Large {
  internal class ClassG3 {
    private readonly ClassG4 _next = new ClassG4();
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

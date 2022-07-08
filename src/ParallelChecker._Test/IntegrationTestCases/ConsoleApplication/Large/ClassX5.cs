namespace Large {
  internal class ClassX5 {
    private readonly ClassX6 _next = new ClassX6();
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

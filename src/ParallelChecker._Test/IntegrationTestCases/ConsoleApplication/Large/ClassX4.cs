namespace Large {
  internal class ClassX4 {
    private readonly ClassX5 _next = new ClassX5();
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

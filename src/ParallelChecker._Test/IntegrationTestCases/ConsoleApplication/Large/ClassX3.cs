namespace Large {
  internal class ClassX3 {
    private readonly ClassX4 _next = new ClassX4();
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

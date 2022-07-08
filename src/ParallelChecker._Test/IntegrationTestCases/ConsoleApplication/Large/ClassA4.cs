namespace Large {
  internal class ClassA4 {
    private readonly ClassA5 _next = new ClassA5();
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

namespace Large {
  internal class ClassA3 {
    private readonly ClassA4 _next = new ClassA4();
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

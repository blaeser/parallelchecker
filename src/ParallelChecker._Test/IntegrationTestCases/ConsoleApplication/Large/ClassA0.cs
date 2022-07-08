namespace Large {
  internal class ClassA0 {
    private readonly ClassA1 _next = new ClassA1();
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

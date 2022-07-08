namespace Large {
  internal class ClassA1 {
    private readonly ClassA2 _next = new ClassA2();
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

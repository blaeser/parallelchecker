namespace Large {
  internal class ClassA2 {
    private readonly ClassA3 _next = new ClassA3();
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

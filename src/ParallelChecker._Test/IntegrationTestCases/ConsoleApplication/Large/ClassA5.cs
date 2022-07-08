namespace Large {
  internal class ClassA5 {
    private readonly ClassA6 _next = new ClassA6();
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

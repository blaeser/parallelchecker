namespace Large {
  internal class ClassA9 {
    private readonly ClassB0 _next = new ClassB0();
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

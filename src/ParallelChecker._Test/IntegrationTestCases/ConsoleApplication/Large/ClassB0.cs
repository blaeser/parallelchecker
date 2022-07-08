namespace Large {
  internal class ClassB0 {
    private readonly ClassB1 _next = new ClassB1();
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

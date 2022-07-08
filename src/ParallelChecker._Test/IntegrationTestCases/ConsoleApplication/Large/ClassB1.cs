namespace Large {
  internal class ClassB1 {
    private readonly ClassB2 _next = new ClassB2();
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

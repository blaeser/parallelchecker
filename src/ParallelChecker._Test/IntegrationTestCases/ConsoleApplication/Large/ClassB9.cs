namespace Large {
  internal class ClassB9 {
    private readonly ClassC0 _next = new ClassC0();
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

namespace Large {
  internal class ClassB5 {
    private readonly ClassB6 _next = new ClassB6();
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

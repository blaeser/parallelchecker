namespace Large {
  internal class ClassB2 {
    private readonly ClassB3 _next = new ClassB3();
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

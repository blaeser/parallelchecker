namespace Large {
  internal class ClassB4 {
    private readonly ClassB5 _next = new ClassB5();
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

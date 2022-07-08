namespace Large {
  internal class ClassB3 {
    private readonly ClassB4 _next = new ClassB4();
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

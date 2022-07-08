namespace Large {
  internal class ClassB6 {
    private readonly ClassB7 _next = new ClassB7();
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

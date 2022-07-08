namespace Large {
  internal class ClassB7 {
    private readonly ClassB8 _next = new ClassB8();
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

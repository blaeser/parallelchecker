namespace Large {
  internal class ClassB8 {
    private readonly ClassB9 _next = new ClassB9();
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

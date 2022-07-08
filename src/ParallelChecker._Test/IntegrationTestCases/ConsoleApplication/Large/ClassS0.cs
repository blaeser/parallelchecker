namespace Large {
  internal class ClassS0 {
    private readonly ClassS1 _next = new ClassS1();
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

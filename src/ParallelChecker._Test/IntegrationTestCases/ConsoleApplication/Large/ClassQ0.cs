namespace Large {
  internal class ClassQ0 {
    private readonly ClassQ1 _next = new ClassQ1();
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

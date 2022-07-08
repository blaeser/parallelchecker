namespace Large {
  internal class ClassQ1 {
    private readonly ClassQ2 _next = new ClassQ2();
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

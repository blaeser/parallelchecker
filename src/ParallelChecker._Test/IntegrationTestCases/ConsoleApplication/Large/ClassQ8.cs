namespace Large {
  internal class ClassQ8 {
    private readonly ClassQ9 _next = new ClassQ9();
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

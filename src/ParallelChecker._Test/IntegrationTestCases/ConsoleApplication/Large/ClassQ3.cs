namespace Large {
  internal class ClassQ3 {
    private readonly ClassQ4 _next = new ClassQ4();
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

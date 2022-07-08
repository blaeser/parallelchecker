namespace Large {
  internal class ClassQ4 {
    private readonly ClassQ5 _next = new ClassQ5();
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

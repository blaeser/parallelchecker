namespace Large {
  internal class ClassQ5 {
    private readonly ClassQ6 _next = new ClassQ6();
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

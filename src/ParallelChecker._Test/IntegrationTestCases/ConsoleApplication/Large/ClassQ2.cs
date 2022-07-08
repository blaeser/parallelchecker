namespace Large {
  internal class ClassQ2 {
    private readonly ClassQ3 _next = new ClassQ3();
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

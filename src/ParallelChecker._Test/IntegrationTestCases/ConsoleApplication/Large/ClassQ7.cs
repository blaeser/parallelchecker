namespace Large {
  internal class ClassQ7 {
    private readonly ClassQ8 _next = new ClassQ8();
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

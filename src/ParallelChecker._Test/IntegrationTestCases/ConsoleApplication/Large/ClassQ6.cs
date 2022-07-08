namespace Large {
  internal class ClassQ6 {
    private readonly ClassQ7 _next = new ClassQ7();
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

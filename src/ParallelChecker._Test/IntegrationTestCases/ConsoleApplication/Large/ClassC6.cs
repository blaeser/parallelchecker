namespace Large {
  internal class ClassC6 {
    private readonly ClassC7 _next = new ClassC7();
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

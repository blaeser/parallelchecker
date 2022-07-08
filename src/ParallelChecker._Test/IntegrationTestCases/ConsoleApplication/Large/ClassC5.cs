namespace Large {
  internal class ClassC5 {
    private readonly ClassC6 _next = new ClassC6();
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

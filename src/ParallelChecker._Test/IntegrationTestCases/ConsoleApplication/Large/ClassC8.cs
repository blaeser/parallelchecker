namespace Large {
  internal class ClassC8 {
    private readonly ClassC9 _next = new ClassC9();
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

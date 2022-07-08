namespace Large {
  internal class ClassU8 {
    private readonly ClassU9 _next = new ClassU9();
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

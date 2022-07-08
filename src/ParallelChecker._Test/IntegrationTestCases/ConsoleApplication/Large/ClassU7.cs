namespace Large {
  internal class ClassU7 {
    private readonly ClassU8 _next = new ClassU8();
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

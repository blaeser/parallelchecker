namespace Large {
  internal class ClassC7 {
    private readonly ClassC8 _next = new ClassC8();
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

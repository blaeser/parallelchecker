namespace Large {
  internal class ClassM7 {
    private readonly ClassM8 _next = new ClassM8();
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

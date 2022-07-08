namespace Large {
  internal class ClassM6 {
    private readonly ClassM7 _next = new ClassM7();
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

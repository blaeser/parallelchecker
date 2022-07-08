namespace Large {
  internal class ClassM9 {
    private readonly ClassN0 _next = new ClassN0();
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

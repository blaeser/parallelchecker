namespace Large {
  internal class ClassN0 {
    private readonly ClassN1 _next = new ClassN1();
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

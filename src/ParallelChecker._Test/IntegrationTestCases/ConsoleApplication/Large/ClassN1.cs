namespace Large {
  internal class ClassN1 {
    private readonly ClassN2 _next = new ClassN2();
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

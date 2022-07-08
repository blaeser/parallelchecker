namespace Large {
  internal class ClassN2 {
    private readonly ClassN3 _next = new ClassN3();
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

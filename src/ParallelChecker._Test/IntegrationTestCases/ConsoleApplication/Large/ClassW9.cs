namespace Large {
  internal class ClassW9 {
    private readonly ClassX0 _next = new ClassX0();
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

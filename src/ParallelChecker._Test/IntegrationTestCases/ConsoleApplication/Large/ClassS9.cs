namespace Large {
  internal class ClassS9 {
    private readonly ClassT0 _next = new ClassT0();
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

namespace Large {
  internal class ClassR9 {
    private readonly ClassS0 _next = new ClassS0();
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

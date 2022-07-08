namespace Large {
  internal class ClassQ9 {
    private readonly ClassR0 _next = new ClassR0();
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

namespace Large {
  internal class ClassR0 {
    private readonly ClassR1 _next = new ClassR1();
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

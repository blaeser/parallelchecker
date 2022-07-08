namespace Large {
  internal class ClassR1 {
    private readonly ClassR2 _next = new ClassR2();
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
